using System.Diagnostics;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Logic;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic.Transformators;

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

    protected override string GenerateHtml(TransformationContext ctx)
    {
        Stopwatch sw = Stopwatch.StartNew();

        XmlSignersDocument signersDocument = (XmlSignersDocument)ctx.SignersDocument;

        try
        {
            string xml = signersDocument.DataAsText();
            string xslt = signersDocument.XsltAsText();

            var result = new XML2HTMLTransformLogic().Transform(xml, xslt);
            return result;
        }
        catch (Exception e)
        {
            string logMessage = $"Error initially transforming {signersDocument.SignersDocumentFile.Name} from XML to HTML: {e.Message}";
            throw new TransformationException(logMessage, ErrorCode.SDK007, e);
        }
    }
}