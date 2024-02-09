using System;
using System.IO;
using System.Xml;

using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Exceptions;
using NemLoginSigningCore.Logging;
using NemLoginSigningCore.Logic;

using static NemLoginSigningCore.Core.SignersDocumentFile;

namespace NemLoginSigningValidation.XMLValidation
{
    /// <summary>
    /// Class for validating XML and XSL files.
    /// Check for syntax errors, version, DtdProcessing etc.
    /// </summary>
    public class XMLValidator : IValidator
    {
        private const string XSL_VERSION_NAME = "version";
        private const string XSL_VERSION = "3.0";
        private const string W3C_XSL_SCHEME = "http://www.w3.org/1999/XSL/Transform";

        private const string XSL_IMPORT = "import";
        private const string XSL_INCLUDE = "include";

        public void Validate(TransformationContext context)
        {
            var logger = LoggerCreator.CreateLogger<XMLValidator>();

            ArgumentNullException.ThrowIfNull(context);

            logger.LogInformation("XMLValidator Validating XML/XSL");

            CheckWellFormedXML(context);
            CheckWellFormedXSL(context);
            CheckXSL(context);
            CheckHTML(context);
        }

        private void CheckHTML(TransformationContext context)
        {
            var xml = ((XmlSignersDocument)context.SignersDocument).SignersDocumentFile.GetDataAsString();
            var xslt = ((XmlSignersDocument)context.SignersDocument).XsltAsText();

            try
            {
                var xHtml = new XML2HTMLTransformLogic().Transform(xml, xslt);

                byte[] ar = System.Text.Encoding.UTF8.GetBytes(xHtml);

                SignersDocumentFile htmlResult = new SignersDocumentFileBuilder()
                                                    .WithName(Path.ChangeExtension(context.SignersDocument.SignersDocumentFile.Name, "html"))
                                                    .WithData(ar)
                                                    .Build();

                IValidator htmlValidator = new HTMLValidator();
                htmlValidator.Validate(new TransformationContext(new HtmlSignersDocument(htmlResult), null, context.SignatureParameters, null));
            }
            catch (Exception)
            {
                throw new ValidationException("Error while transforming XML to HTML", ErrorCode.SDK010);
            }
        }

        private void CheckWellFormedXML(TransformationContext context)
        {
            try
            {
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit };

                using (MemoryStream memoryStream = new MemoryStream(context.SignersDocument.SignersDocumentFile.GetData()))
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

        private void CheckWellFormedXSL(TransformationContext context)
        {
            try
            {
                XmlSignersDocument xmlSignersDocument = context.SignersDocument as XmlSignersDocument;

                ArgumentNullException.ThrowIfNull(context.SignersDocument);

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
            var xmlSignersDocument = ctx.SignersDocument as XmlSignersDocument;
            var document = GetDocument(xmlSignersDocument.XsltFile.GetData());

            ValidateIsVersion30(document);
            IsImportOrInclude(document);
        }

        private void IsImportOrInclude(XmlDocument document)
        {
            foreach (var child in document.ChildNodes)
            {
                var node = (XmlNode)child;
                IsImportOrInclude(node);
            }
        }

        private void IsImportOrInclude(XmlNode xmlNode)
        {
            foreach (var child in xmlNode.ChildNodes)
            {
                var node = (XmlNode)child;

                if (node.NamespaceURI == W3C_XSL_SCHEME && (node.LocalName == XSL_IMPORT || node.LocalName == XSL_INCLUDE))
                {
                    throw new ValidationException("Import or Include in XSL schema is not allowed", ErrorCode.SDK010);
                }

                IsImportOrInclude(node);
            }
        }

        public void ValidateIsVersion30(XmlDocument document)
        {
            ArgumentNullException.ThrowIfNull(document);

            bool version = false;
            bool scheme = false;

            var attributes = document.DocumentElement.Attributes;

            foreach (var xmlAttribute in attributes)
            {
                XmlAttribute attribute = (XmlAttribute)xmlAttribute;

                if (attribute.LocalName == XSL_VERSION_NAME && attribute.Value == XSL_VERSION)
                {
                    version = true;
                }

                if (attribute.Value == W3C_XSL_SCHEME)
                {
                    scheme = true;
                }
            }

            if ((version && scheme) == false)
            {
                throw new ValidationException("XSL schema must be version 3.0", ErrorCode.SDK010);
            }
        }
    }
}
