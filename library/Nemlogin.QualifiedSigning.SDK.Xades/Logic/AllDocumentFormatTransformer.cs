using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Logic;
using Nemlogin.QualifiedSigning.SDK.Xades.GeneratedSources;
using Nemlogin.QualifiedSigning.SDK.Xades.Utilities;

namespace Nemlogin.QualifiedSigning.SDK.Xades.Logic;

/// <summary>
/// Implementation of the ITransformer interface. 
/// Handles all the preliminary formats (XML, HTML, TXT, PDF) -> XML transformation.
/// </summary>
public class AllDocumentFormatTransformer : ITransformer
{
    private string CreateSignTextTypeId() => $"id-" + Guid.NewGuid();

    public bool CanTransform(Transformation transformation)
    {
        if (transformation == null)
        {
            throw new ArgumentNullException(nameof(transformation));
        }

        return transformation.SignatureFormat == SignatureFormat.XAdES;
    }

    public void Transform(TransformationContext ctx)
    {
        if (ctx == null)
        {
            throw new ArgumentNullException(nameof(ctx));
        }

        SignTextType signTextType = new SignTextType { id = CreateSignTextTypeId() };

        switch (ctx.SignersDocument.DocumentFormat)
        {
            case DocumentFormat.TEXT:
                signTextType.WithPlainText(CreatePlainTextType(ctx.SignersDocument));
                break;
            case DocumentFormat.HTML:
                signTextType.WithHtmlDocument(CreateHtmlDocument(ctx.SignersDocument));
                break;
            case DocumentFormat.XML:
                signTextType.WithXmlDocument(CreateXmlDocument(ctx.SignersDocument));
                break;
            case DocumentFormat.PDF:
                signTextType.WithPdfDocument(CreatePdfDocument(ctx.SignersDocument));
                break;
        }

        if (ctx.SignersDocument.SignProperties != null && ctx.SignersDocument.SignProperties.Any())
        {
            signTextType.WithProperties(ctx.SignersDocument.SignProperties);
        }

        SignedDocumentType signedDocumentType = new SignedDocumentType { SignText = signTextType };

        try
        {
            byte[] serializedXml = XmlSerializer.Serialize(signedDocumentType);
            ctx.DataToBeSigned = new XadesDataToBeSigned(serializedXml, ctx.SignersDocument.SignersDocumentFile.Name);
        }
        catch (Exception e)
        {
            throw new TransformationException("Error in serialization of XML", ErrorCode.SDK009, e);
        }
    }

    private PlainTextType CreatePlainTextType(SignersDocument signersDocument)
    {
        if (signersDocument == null)
        {
            throw new ArgumentNullException(nameof(signersDocument));
        }

        PlainTextType plainTextType = new()
        {
            Document = signersDocument.SignersDocumentFile.GetData(),
            Rendering = new RenderingType
            {
                UseMonoSpaceFont = ((PlainTextSignersDocument)signersDocument).UseMonoSpaceFont
            }
        };

        return plainTextType;
    }

    private XMLDocumentType CreateXmlDocument(SignersDocument signersDocument)
    {
        XMLDocumentType xmlDocumentType = new()
        {
            Document = signersDocument.SignersDocumentFile.GetData(),
            Transformation = ((XmlSignersDocument)signersDocument).XsltFile.GetData()
        };

        return xmlDocumentType;
    }

    private HTMLDocumentType CreateHtmlDocument(SignersDocument signersDocument)
    {
        HTMLDocumentType htmlDocumentType = new()
        {
            Document = signersDocument.SignersDocumentFile.GetData()
        };

        return htmlDocumentType;
    }

    private PDFDocumentType CreatePdfDocument(SignersDocument signersDocument)
    {
        PDFDocumentType pdfDocumentType = new()
        {
            Document = signersDocument.SignersDocumentFile.GetData()
        };

        return pdfDocumentType;
    }
}