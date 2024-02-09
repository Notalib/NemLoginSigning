using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Exceptions;
using NemLoginSigningCore.Format;
using NemLoginSigningCore.Logging;
using NemLoginSigningCore.Logic;
using Org.BouncyCastle.Security;

namespace NemLoginSigningPades.Logic
{
    /// <summary>
    /// Class for presign signature handling of PDF files.
    /// PresignDocument - handles the following steps.
    /// 1. Check document for existing signature
    /// 2. Presign document with template signature dictionary with fixed lenght content
    /// 3. Set data to be signed to the document with the signature
    /// 4. Update the DTBS signed info with the base64 encoded signedinfo part.
    /// </summary>
    public class PdfSignatureStamper : ISignatureStamper
    {
        public const int SIGNATURE_SIZE = 16384;
        public const string SIGNATURE_TYPE = "Sig";
        public const string SIGNATURE_NAME = "NemLog-In Signing SDK";
        public const int NO_CHANGE_PERMITTED = 1;

        public static readonly PdfName SIGNATURE_DEFAULT_FILTER = PdfName.ADOBE_PPKLITE;
        public static readonly PdfName SIGNATURE_DEFAULT_SUBFILTER = PdfName.ETSI_CADES_DETACHED;

        public bool CanSign(SignatureFormat signatureFormat)
        {
            return signatureFormat == SignatureFormat.PAdES;
        }

        public void PresignDocument(TransformationContext context)
        {
            byte[] data = context.DataToBeSigned.GetData();

            CheckForExistingSignatures(data);

            PresignResult preSignResult = PreSignDocumentAndReturnDigest(context, context.DataToBeSigned.GetData());

            // Update the DTBS PDF document
            context.DataToBeSigned = new PadesDataToBeSigned(preSignResult.SignedResult, context.DataToBeSigned.FileName);

            // Update the signature parameters with the CMS SignerInfo element
            string signerInfo = Convert.ToBase64String(preSignResult.Signature);
            context.UpdateDtbsSignedInfo(signerInfo);
        }

        /// <summary>
        /// Use iTextSharp to check if PdfDocument allready has existing signatures
        /// If any signatures exist we throw a TransformationExeption.
        /// </summary>
        /// <param name="pdfDocument"></param>
        private void CheckForExistingSignatures(byte[] pdfDocument)
        {
            ILogger logger = LoggerCreator.CreateLogger<PdfSignatureStamper>();

            try
            {
                PdfReader reader = new(pdfDocument);
                AcroFields acroFields = reader.AcroFields;

                IEnumerable<String> signatureNames = acroFields.GetSignatureNames().Cast<string>().ToList();

                if (signatureNames.Any())
                {
                    throw new TransformationException($"DTBS PDF already contains {signatureNames.Count()} Signature Dictionaries", ErrorCode.SDK005);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error extracting DTBS PDF Signature Dictionaries. {Message}", e.Message);
                throw new TransformationException("Error extracting DTBS PDF Signature Dictionaries", ErrorCode.SDK005, e);
            }
        }

        private PresignResult PreSignDocumentAndReturnDigest(TransformationContext ctx, byte[] pdfDocument)
        {
            SignatureKeys signatureKeys = new SignatureKeysProducer().CreateSignatureKeys();

            return StampSignature(pdfDocument, signatureKeys, ctx);
        }

        private PresignResult StampSignature(byte[] pdfDocument, SignatureKeys signatureKeys, TransformationContext ctx)
        {
            MemoryStream ms = new MemoryStream();
            PdfReader reader = new PdfReader(pdfDocument);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, ms, '\0', null, true);

            PdfSignatureAppearance appearance = CreateSignatureAppearance(stamper);

            TemplateSignatureContainer templateSignatureContainer = CreateTemplateSignatureContainer(ctx, signatureKeys);

            MakeSignature.SignExternalContainer(appearance, templateSignatureContainer, SIGNATURE_SIZE);

            return new PresignResult(ms.ToArray(), templateSignatureContainer.Digest);
        }

        private TemplateSignatureContainer CreateTemplateSignatureContainer(TransformationContext ctx, SignatureKeys signatureKeys)
        {
            Collection<Org.BouncyCastle.X509.X509Certificate> certificateCollection = [DotNetUtilities.FromX509Certificate(signatureKeys.X509Certificate2)];

            Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey = DotNetUtilities.GetKeyPair(signatureKeys.PrivateKey).Private;

            return new TemplateSignatureContainer(ctx, privateKey, certificateCollection);
        }

        private PdfSignatureAppearance CreateSignatureAppearance(PdfStamper pdfStamper)
        {
            PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
            signatureAppearance.SignDate = DateTime.Now;
            signatureAppearance.SetVisibleSignature(new Rectangle(0, 0, 0, 0), 1, SIGNATURE_NAME);
            signatureAppearance.CryptoDictionary = CreatePdfDictionary();

            return signatureAppearance;
        }

        private PdfDictionary CreatePdfDictionary()
        {
            PdfDictionary pdfDictionary = new PdfSignature(SIGNATURE_DEFAULT_FILTER, SIGNATURE_DEFAULT_SUBFILTER);
            pdfDictionary.Put(PdfName.TYPE, new PdfName(SIGNATURE_TYPE));
            pdfDictionary.Put(PdfName.CREATIONDATE, new PdfDate(DateTime.Now));
            pdfDictionary.Put(PdfName.NAME, new PdfName(SIGNATURE_NAME));

            return pdfDictionary;
        }
    }
}
