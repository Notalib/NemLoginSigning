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
            ArgumentNullException.ThrowIfNull(transformation);

            return transformation.SignatureFormat == SignatureFormat.XAdES;
        }

        public void Transform(TransformationContext transformationContext, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(transformationContext);

            SignTextType signTextType = new SignTextType { id = CreateSignTextTypeId() };

            switch (transformationContext.SignersDocument.DocumentFormat)
            {
                case DocumentFormat.TEXT:
                    signTextType.WithPlainText(CreatePlainTextType(transformationContext.SignersDocument));
                    break;
                case DocumentFormat.HTML:
                    signTextType.WithHTMLDocument(CreateHTMLDocument(transformationContext.SignersDocument));
                    break;
                case DocumentFormat.XML:
                    signTextType.WithXMLDocument(CreateXMLDocument(transformationContext.SignersDocument));
                    break;
                case DocumentFormat.PDF:
                    signTextType.WithPDFDocument(CreatePDFDocument(transformationContext.SignersDocument));
                    break;
            }

            if (transformationContext.SignersDocument.SignProperties != null && transformationContext.SignersDocument.SignProperties.Any())
            {
                signTextType.WithProperties(transformationContext.SignersDocument.SignProperties);
            }

            SignedDocumentType signedDocumentType = new SignedDocumentType { SignText = signTextType };

            try
            {
                var serializedXML = XMLSerializer.Serialize(signedDocumentType);
                transformationContext.DataToBeSigned = new XadesDataToBeSigned(serializedXML, transformationContext.SignersDocument.SignersDocumentFile.Name);
            }
            catch (Exception e)
            {
                throw new TransformationException("Error in serialization of XML", ErrorCode.SDK009, e);
            }
        }

        private PlainTextType CreatePlainTextType(SignersDocument signersDocument)
        {
            ArgumentNullException.ThrowIfNull(signersDocument);

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