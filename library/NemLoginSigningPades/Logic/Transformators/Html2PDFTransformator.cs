using System;
using System.Diagnostics;
using System.IO;

using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Exceptions;
using NemLoginSigningCore.Format;
using NemLoginSigningCore.Logic;

namespace NemLoginSigningPades.Logic.Transformators
{
    /// <summary>
    /// Implementation of the ITransformator interface for handling transformations from
    /// HTML => PDF using iTextSharp &amp; HtmlAgilityPack
    /// </summary>
    public class Html2PDFTransformator : ITransformator
    {
        public bool CanTransform(Transformation transformation)
        {
            return transformation.SignatureFormat == SignatureFormat.PAdES &&
                transformation.SdFormat == DocumentFormat.HTML;
        }

        public void Transform(TransformationContext transformationContext, ILogger logger)
        {
            Stopwatch sw = Stopwatch.StartNew();
            SignersDocument signersDocument = transformationContext.SignersDocument;
            Html2PDFGenerator html2PDFGenerator = new Html2PDFGenerator();
            TransformationPropertiesHandler propertiesHandler = new TransformationPropertiesHandler(transformationContext.TransformationProperties);

            logger.LogInformation("Start transforming {Name} from HTML to PDF", signersDocument.SignersDocumentFile.Name);

            string html = signersDocument.SignersDocumentFile.GetDataAsString();

            try
            {
                var pdfDocument = html2PDFGenerator.GeneratePDFDocument(html, propertiesHandler);

                transformationContext.DataToBeSigned = new PadesDataToBeSigned(pdfDocument, Path.ChangeExtension(signersDocument.SignersDocumentFile.Name, "pdf"));

                logger.LogInformation("Transformed {Name} from HTML to PDF in {MilliSecond} ms", signersDocument.SignersDocumentFile.Name, sw.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error transforming {Name} from HTML to PDF: {Message}", signersDocument.SignersDocumentFile.Name, e.Message);
                throw new TransformationException($"Error transforming {signersDocument.SignersDocumentFile.Name} from HTML to PDF", ErrorCode.SDK007, e);
            }
        }
    }
}