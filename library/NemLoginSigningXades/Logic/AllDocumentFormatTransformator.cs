using System;
using System.Linq;

using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Exceptions;
using NemLoginSigningCore.Logic;
using NemLoginSigningCore.Format;
using NemLoginSigningXades.GeneratedSources;
using NemLoginSigningXades.Util;

namespace NemLoginSigningXades.Logic
{
    /// <summary>
    /// Implementation of the ITransformator interface.
    /// Handles all the preliminary formats (XML, HTML, TXT, PDF) -> XML transformation.
    /// </summary>
    public class AllDocumentFormatTransformator : ITransformator
    {
        private string CreateSignTextTypeId() => $"id-" + Guid.NewGuid();

        public bool CanTransform(Transformation transformation)
        {
            if (transformation == null)
            {
                throw new ArgumentNullException(nameof(Transform));
            }

            return transformation.SignatureFormat == SignatureFormat.XAdES;
        }

        public void Transform(TransformationContext ctx, ILogger logger)
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
                    signTextType.WithHTMLDocument(CreateHTMLDocument(ctx.SignersDocument));
                    break;
                case DocumentFormat.XML:
                    signTextType.WithXMLDocument(CreateXMLDocument(ctx.SignersDocument));
                    break;
                case DocumentFormat.PDF:
                    signTextType.WithPDFDocument(CreatePDFDocument(ctx.SignersDocument));
                    break;
            }

            if (ctx.SignersDocument.SignProperties != null && ctx.SignersDocument.SignProperties.Any())
            {
                signTextType.WithProperties(ctx.SignersDocument.SignProperties);
            }

            SignedDocumentType signedDocumentType = new SignedDocumentType { SignText = signTextType };

            try
            {
                var serializedXML = XMLSerializer.Serialize(signedDocumentType);
                ctx.DataToBeSigned = new XadesDataToBeSigned(serializedXML, ctx.SignersDocument.SignersDocumentFile.Name);
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

            PlainTextType plainTextType = new PlainTextType();
            plainTextType.Document = signersDocument.SignersDocumentFile.GetData();

            plainTextType.Rendering = new RenderingType();
            plainTextType.Rendering.UseMonoSpaceFont = ((PlainTextSignersDocument)signersDocument).UseMonoSpaceFont;

            return plainTextType;
        }

        private XMLDocumentType CreateXMLDocument(SignersDocument signersDocument)
        {
            XMLDocumentType xmlDocumentType = new XMLDocumentType();
            xmlDocumentType.Document = signersDocument.SignersDocumentFile.GetData();
            xmlDocumentType.Transformation = ((XmlSignersDocument)signersDocument).XsltFile.GetData();

            return xmlDocumentType;
        }

        private HTMLDocumentType CreateHTMLDocument(SignersDocument signersDocument)
        {
            HTMLDocumentType htmlDocumentType = new HTMLDocumentType();
            htmlDocumentType.Document = signersDocument.SignersDocumentFile.GetData();

            return htmlDocumentType;
        }

        private PDFDocumentType CreatePDFDocument(SignersDocument signersDocument)
        {
            PDFDocumentType pdfDocumentType = new PDFDocumentType();
            pdfDocumentType.Document = signersDocument.SignersDocumentFile.GetData();

            return pdfDocumentType;
        }
    }
}