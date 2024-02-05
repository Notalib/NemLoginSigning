using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Exceptions;
using NemLoginSigningCore.Format;
using NemLoginSigningCore.Logic;

using static NemLoginSigningCore.Core.SignatureParameters;
using static NemLoginSigningCore.Core.SignersDocumentFile;

namespace NemLoginSigningPades.Logic.Transformators
{
    /// <summary>
    /// Baseclass for PDF transformations which are inherited from the specific transformation classes
    /// </summary>
    public abstract class PdfFormatTransformationService : ITransformator
    {
        public abstract bool CanTransform(Transformation transformation);

        protected abstract string GenerateHTML(TransformationContext ctx, ILogger logger);

        public void Transform(TransformationContext ctx, ILogger logger)
        {
            Stopwatch sw = Stopwatch.StartNew();

            SignersDocument signersDocument = ctx.SignersDocument;

            logger.LogInformation("Start transforming {Name} from {Format} to PDF", signersDocument.SignersDocumentFile.Name, signersDocument.DocumentFormat);

            // Step 1: Generate HTML by transforming the XML using the included XSLT
            string html = GenerateHTML(ctx, logger);

            // Step 2: Generate PDF from the HTML
            try
            {
                string fileName = ctx.SignersDocument.SignersDocumentFile.Name;
                byte[] pdf = GeneratePDF(html, fileName, ctx.TransformationProperties, logger);

                DataToBeSigned dtbs = new PadesDataToBeSigned(pdf, Path.ChangeExtension(fileName, "pdf"));

                ctx.DataToBeSigned = dtbs;

                logger.LogInformation("Transformed {Name} from {Format} to PDF in {MilliSeconds} ms", signersDocument.SignersDocumentFile.Name, signersDocument.DocumentFormat, sw.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error transforming from {Name} from {Format} to PDF: {Message}", signersDocument.SignersDocumentFile.Name, signersDocument.DocumentFormat, e.Message);
                throw new TransformationException($"Error transforming from {signersDocument.SignersDocumentFile.Name} from {signersDocument.DocumentFormat} to PDF: {e.Message}", ErrorCode.SDK007, e);
            }
        }

        public byte[] GeneratePDF(string html, string name, TransformationProperties transformationProperties, ILogger logger)
        {
            Transformation transformation = ValidTransformation.GetTransformation(DocumentFormat.HTML, SignatureFormat.PAdES);

            var transformator = TransformatorFactory.Create(transformation);

            SignersDocumentFile signersDocumentFile = new SignersDocumentFileBuilder()
                .WithData(Encoding.UTF8.GetBytes(html))
                .WithName(name)
                .Build();

            SignatureParameters signatureParameters = new SignatureParametersBuilder()
                .WithValidTransformation(transformation)
                .Build();

            TransformationContext ctx = new TransformationContext(
                new HtmlSignersDocument(signersDocumentFile),
                null,
                signatureParameters,
                transformationProperties);

            transformator.Transform(ctx, logger);

            return ctx.DataToBeSigned.GetData();
        }
    }
}
