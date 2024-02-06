using System;
using System.Diagnostics;

using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Exceptions;
using NemLoginSigningCore.Format;
using NemLoginSigningCore.Logic;

namespace NemLoginSigningPades.Logic.Transformators
{
    /// <summary>
    /// Implementation of the ITransformator interface.
    /// Handles XML -> PDF conversion by generating HTML and then from HTML -> PDF.
    /// </summary>
    public class XML2PDFTransformator : PdfFormatTransformationService
    {
        public override bool CanTransform(Transformation transformation)
        {
            return transformation.SignatureFormat == SignatureFormat.PAdES &&
            transformation.SdFormat == DocumentFormat.XML;
        }

        protected override string GenerateHTML(TransformationContext ctx, ILogger logger)
        {
            Stopwatch sw = Stopwatch.StartNew();

            XmlSignersDocument signersDocument = (XmlSignersDocument)ctx.SignersDocument;

            try
            {
                string xml = signersDocument.DataAsText();
                string xslt = signersDocument.XsltAsText();

                var result = new XML2HTMLTransformLogic().Transform(xml, xslt);

                logger.LogInformation("Initially transformed {Name} from XML to HTML in {MilliSeconds} ms", signersDocument.SignersDocumentFile.Name, sw.ElapsedMilliseconds);

                return result;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error initially transforming {Name} from XML to HTML: {Message}", signersDocument.SignersDocumentFile.Name, e.Message);
                throw new TransformationException(e.Message, ErrorCode.SDK007, e);
            }
        }
    }
}
