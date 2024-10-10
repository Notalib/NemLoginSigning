using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Logic;
using Org.BouncyCastle.Security;
using PdfSharp;
using PdfSharp.Pdf.IO;
using PdfAnnotation = PdfSharp.Pdf.Annotations.PdfAnnotation;
using PdfDocument = PdfSharp.Pdf.PdfDocument;
using PdfPage = PdfSharp.Pdf.PdfPage;
using PdfReader = PdfSharp.Pdf.IO.PdfReader;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Signatures;
using PdfSharp.Snippets.Font;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic;

/// <summary>
/// Class for preSign signature handling of PDF files.
/// PreSignDocument - handles the following steps.
/// 1. Check document for existing signature
/// 2. PreSign document with template signature dictionary with fixed length content
/// 3. Set data to be signed to the document with the signature
/// 4. Update the DTBS signed info with the base64 encoded signedInfo part.
/// </summary>
public class PdfSignatureStamperV2 : ISignatureStamper
{
    public const int SIGNATURE_SIZE = 16384;
    public const string SIGNATURE_TYPE = "/Sig";
    public const string SIGNATURE_NAME = "NemLog-In Signing SDK";
    private const int NO_CHANGE_PERMITTED = 1;


    // public static readonly PdfName SIGNATURE_DEFAULT_FILTER = PdfName.ADOBE_PPKLITE;
    // public static readonly PdfName SIGNATURE_DEFAULT_SUBFILTER = PdfName.ETSI_CADES_DETACHED;

    public bool CanSign(SignatureFormat signatureFormat)
    {
        return signatureFormat == SignatureFormat.PAdES;
    }

    public void PreSignDocument(TransformationContext ctx)
    {
        byte[] data = ctx.DataToBeSigned.GetData();

        CheckForExistingSignatures(data);

        if (Capabilities.Build.IsCoreBuild && GlobalFontSettings.FontResolver is null)
            GlobalFontSettings.FontResolver = new FailsafeFontResolver();

        PreSignResult preSignResult = PreSignDocumentAndReturnDigest(ctx, ctx.DataToBeSigned.GetData());

        // Update the DTBS PDF document
        ctx.DataToBeSigned = new PadesDataToBeSigned(preSignResult.SignedResult, ctx.DataToBeSigned.FileName);
        //
        // // Update the signature parameters with the CMS SignerInfo element
        string signerInfo = Convert.ToBase64String(preSignResult.Signature);
        ctx.UpdateDtbsSignedInfo(signerInfo);
    }

    /// <summary>
    /// Use PdfSharp to check if PdfDocument already has existing signatures
    /// If any signatures exist we throw a TransformationException.
    /// </summary>
    /// <param name="pdfDocument"></param>
    private void CheckForExistingSignatures(byte[] pdfDocument)
    {
        List<string> signatureNames = new();
        // Load the PDF document

        using MemoryStream ms = new(pdfDocument);
        PdfDocument document = PdfReader.Open(ms, PdfDocumentOpenMode.Import);

        // Check each page for digital signatures
        for (int pageIndex = 0; pageIndex < document.PageCount; pageIndex++)
        {
            PdfPage page = document.Pages[pageIndex];

            try
            {
                // Check annotations on the page
                for (int index = 0; index < page.Annotations.Count; index++)
                {
                    PdfAnnotation annotation = page.Annotations[index];
                    if (annotation.Elements.GetString("/Subtype") == "/Widget" &&
                        annotation.Elements.GetString("/FT") == SIGNATURE_TYPE)
                    {
                        signatureNames.Add(annotation.Elements.GetString("/T"));
                    }
                }

                if (signatureNames.Any())
                {
                    throw new TransformationException($"DTBS PDF already contains {signatureNames.Count()} Signature Dictionaries", ErrorCode.SDK005);
                }
            }
            catch (Exception e)
            {
                throw new TransformationException($"Error extracting DTBS PDF Signature Dictionaries. {e.Message}", ErrorCode.SDK005, e);
            }
        }

        // Dispose the document
        document.Dispose();
    }

    private static (PdfSignatureHandler, byte[]) SignPdf(PdfDocument pdfDocument, SignatureKeys signatureKeys)
    {
        PdfSignatureOptions options = new()
        {
            ContactInfo = SIGNATURE_NAME,
            Location = null,
            Reason = null,
            Rectangle = new XRect(0, 0, 0, 0),
            PageIndex = pdfDocument.PageCount - 1,
            AppearanceHandler = new SignAppearanceHandler()
        };
        Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey = DotNetUtilities.GetKeyPair(signatureKeys.PrivateKey).Private;
        PdfSignatureHandler pdfSignatureHandler = new(new SigningSigner(GetCertificate(signatureKeys.X509Certificate2), privateKey), options);
        pdfSignatureHandler.AttachToDocument(pdfDocument);

        MemoryStream stream = new();
        pdfDocument.Save(stream, false);
        // pdfDocument.Save("Signed-pdf.pdf");

        return (pdfSignatureHandler, stream.ToArray());
    }

    private PreSignResult PreSignDocumentAndReturnDigest(TransformationContext ctx, byte[] pdfDocument)
    {
        SignatureKeys signatureKeys = new SignatureKeysProducer().CreateSignatureKeys();

        return StampSignature(pdfDocument, signatureKeys, ctx);
    }

    private PreSignResult StampSignature(byte[] pdfDocument, SignatureKeys signatureKeys, TransformationContext ctx)
    {
        PdfDocument clonedDocument = new();
        using MemoryStream ms = new(pdfDocument);
        // Load the PDF document from the MemoryStream
        PdfDocument document = PdfReader.Open(ms, PdfDocumentOpenMode.Import);

        // Clone the document to ensure the MemoryStream is not closed when the document is closed
        foreach (PdfPage page in document.Pages)
        {
            clonedDocument.AddPage(page);
        }

        TemplateSignatureContainer templateSignatureContainer = CreateTemplateSignatureContainer(ctx, signatureKeys);

        (PdfSignatureHandler, byte[]) signatureHandler = SignPdf(clonedDocument, signatureKeys);

        templateSignatureContainer.Sign(ms);
        return new PreSignResult(signatureHandler.Item2, signatureHandler.Item1.Digest);
    }

    private TemplateSignatureContainer CreateTemplateSignatureContainer(TransformationContext ctx, SignatureKeys signatureKeys)
    {
        Collection<Org.BouncyCastle.X509.X509Certificate> certificateCollection = new Collection<Org.BouncyCastle.X509.X509Certificate> { DotNetUtilities.FromX509Certificate(signatureKeys.X509Certificate2) };

        Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey = DotNetUtilities.GetKeyPair(signatureKeys.PrivateKey).Private;

        return new TemplateSignatureContainer(ctx, privateKey, certificateCollection);
    }

    private static Tuple<X509Certificate2, X509Certificate2Collection> GetCertificate(X509Certificate2 certificate)
    {
        byte[] rawData = certificate.RawData;

        X509Certificate2Collection collection = new X509Certificate2Collection();
        collection.Import(rawData, null, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);


        return Tuple.Create(certificate, collection);
    }

}