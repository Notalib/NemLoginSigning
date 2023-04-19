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
    /// HTML => PDF using iTextSharp & HtmlAgilityPack
    /// </summary>
    public class Html2PDFTransformator : ITransformator
    {
        public bool CanTransform(Transformation transformation)
        {
            return transformation.SignatureFormat == SignatureFormat.PAdES &&
                transformation.SdFormat == DocumentFormat.HTML;
        }

        public void Transform(TransformationContext ctx, ILogger logger)
        {
            Stopwatch sw = Stopwatch.StartNew();
            SignersDocument signersDocument = ctx.SignersDocument;
            Html2PDFGenerator html2PDFGenerator = new Html2PDFGenerator();
            TransformationPropertiesHandler propertiesHandler = new TransformationPropertiesHandler(ctx.TransformationProperties);

            logger.LogInformation($"Start transforming {signersDocument.SignersDocumentFile.Name} from HTML to PDF");

            string html = signersDocument.SignersDocumentFile.GetDataAsString();

            try
            {
                var pdfDocument = html2PDFGenerator.GeneratePDFDocument(html, propertiesHandler);

                ctx.DataToBeSigned = new PadesDataToBeSigned(pdfDocument, Path.ChangeExtension(signersDocument.SignersDocumentFile.Name, "pdf"));

                logger.LogInformation($"Transformed {signersDocument.SignersDocumentFile.Name} from HTML to PDF in {sw.ElapsedMilliseconds} ms");
            }
            catch (Exception e)
            {
                logger.LogError($"Error transforming {signersDocument.SignersDocumentFile.Name} from HTML to PDF: {e.Message}");
                throw new TransformationException("Error transforming {signersDocument.SignersDocumentFile.Name} from HTML to PDF", ErrorCode.SDK007, e);
            }
        }
    }
}