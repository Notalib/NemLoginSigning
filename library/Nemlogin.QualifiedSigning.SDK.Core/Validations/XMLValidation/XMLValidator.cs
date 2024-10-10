using System.Xml;

using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Logic;

namespace Nemlogin.QualifiedSigning.SDK.Core.Validations.XMLValidation;

/// <summary>
/// Class for validating XML and XSL files. 
/// Check for syntax errors, version, DtdProcessing etc.
/// </summary>
public class XMLValidator : IValidator
{
    private readonly string XSL_VERSION_NAME = "version";
    private readonly string XSL_VERSION = "3.0";
    private readonly string W3C_XSL_SCHEME = "http://www.w3.org/1999/XSL/Transform";

    private readonly string XSL_IMPORT = "import";
    private readonly string XSL_INCLUDE = "include";

    public void Validate(TransformationContext ctx)
    {
        if (ctx == null)
        {
            throw new ArgumentException(nameof(ctx));
        }

        CheckWellFormedXML(ctx);
        CheckWellFormedXSL(ctx);
        CheckXSL(ctx);
        CheckHTML(ctx);
    }

    public bool CanValidate(DocumentFormat format)
    {
        return format == DocumentFormat.XML;
    }

    private void CheckHTML(TransformationContext ctx)
    {
        string xml = ((XmlSignersDocument)ctx.SignersDocument).SignersDocumentFile.GetDataAsString();
        string xslt = ((XmlSignersDocument)ctx.SignersDocument).XsltAsText();

        try
        {
            string xHtml = new XML2HTMLTransformLogic().Transform(xml, xslt);

            byte[] ar = System.Text.Encoding.UTF8.GetBytes(xHtml);

            SignersDocumentFile htmlResult = new SignersDocumentFile.SignersDocumentFileBuilder()
                                                .WithName(Path.ChangeExtension(ctx.SignersDocument.SignersDocumentFile.Name, "html"))
                                                .WithData(ar)
                                                .Build();

            IValidator htmlValidator = new HTMLValidator();
            htmlValidator.Validate(new TransformationContext(new HtmlSignersDocument(htmlResult), null, ctx.SignatureParameters, null));
        }
        catch (Exception)
        {
            throw new ValidationException("Error while transforming XML to HTML", ErrorCode.SDK010);
        }
    }

    private void CheckWellFormedXML(TransformationContext ctx)
    {
        try
        {
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit };

            using (MemoryStream memoryStream = new MemoryStream(ctx.SignersDocument.SignersDocumentFile.GetData()))
            using (XmlReader xmlReader = XmlReader.Create(memoryStream, xmlReaderSettings))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlReader);
            }
        }
        catch (XmlException e)
        {
            throw new ValidationException($"Error validation XML. {e.Message}", ErrorCode.SDK010, e);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void CheckWellFormedXSL(TransformationContext ctx)
    {
        try
        {
            XmlSignersDocument xmlSignersDocument = ctx.SignersDocument as XmlSignersDocument;

            if (xmlSignersDocument == null)
            {
                throw new NullReferenceException($"nameof(xmlSignersDocument) is null");
            }

            using (MemoryStream memoryStream = new MemoryStream(xmlSignersDocument.XsltFile.GetData()))
            using (XmlReader xmlReader = XmlReader.Create(memoryStream, new XmlReaderSettings()))
            {
                XmlDocument document = new XmlDocument();
                document.Load(xmlReader);
            }
        }
        catch (XmlException e)
        {
            throw new ValidationException($"Error validation XSL. {e.Message}", ErrorCode.SDK010, e);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private XmlDocument GetDocument(byte[] data)
    {
        using (MemoryStream memoryStream = new MemoryStream(data))
        using (XmlReader xmlReader = XmlReader.Create(memoryStream, new XmlReaderSettings()))
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlReader);

            return document;
        }
    }

    private void CheckXSL(TransformationContext ctx)
    {
        XmlSignersDocument xmlSignersDocument = ctx.SignersDocument as XmlSignersDocument;
        XmlDocument document = GetDocument(xmlSignersDocument.XsltFile.GetData());

        ValidateIsVersion30(document);
        IsImportOrInclude(document);

    }

    private void IsImportOrInclude(XmlDocument document)
    {
        foreach (object child in document.ChildNodes)
        {
            XmlNode node = (XmlNode)child;
            IsImportOrInclude(node);
        }
    }

    private void IsImportOrInclude(XmlNode xmlNode)
    {
        foreach (object child in xmlNode.ChildNodes)
        {
            XmlNode node = (XmlNode)child;

            if (node.NamespaceURI == W3C_XSL_SCHEME && (node.LocalName == XSL_IMPORT || node.LocalName == XSL_INCLUDE))
            {
                throw new ValidationException("Import or Include in XSL schema is not allowed", ErrorCode.SDK010);
            }

            IsImportOrInclude(node);
        }
    }

    public void ValidateIsVersion30(XmlDocument document)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));

        bool version = false;
        bool scheme = false;

        XmlAttributeCollection attributes = document.DocumentElement.Attributes;

        foreach (object xmlAttribute in attributes)
        {
            XmlAttribute attribute = (XmlAttribute)xmlAttribute;

            if (attribute.LocalName == XSL_VERSION_NAME && attribute.Value == XSL_VERSION)
                version = true;

            if (attribute.Value == W3C_XSL_SCHEME)
                scheme = true;
        }

        if ((version && scheme) == false)
        {
            throw new ValidationException("XSL schema must be version 3.0", ErrorCode.SDK010);
        }
    }
}