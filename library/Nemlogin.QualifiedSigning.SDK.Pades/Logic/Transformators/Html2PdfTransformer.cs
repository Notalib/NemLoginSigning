using System.Diagnostics;

using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Logic;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic.Transformators;

/// <summary>
/// Implementation of the ITransformator interface for handling transformations from
/// HTML => PDF using PdfSharp & HtmlAgilityPack
/// </summary>
public class Html2PdfTransformer : ITransformer
{
    public bool CanTransform(Transformation transformation)
    {
        return transformation.SignatureFormat == SignatureFormat.PAdES &&
               transformation.SdFormat == DocumentFormat.HTML;
    }

    public void Transform(TransformationContext transformationContext)
    {
        Stopwatch sw = Stopwatch.StartNew();
        SignersDocument signersDocument = transformationContext.SignersDocument;
        Html2PdfGeneratorV2 html2PdfGenerator = new();
        TransformationPropertiesHandler propertiesHandler = new TransformationPropertiesHandler(transformationContext.TransformationProperties);

        string html = signersDocument.SignersDocumentFile.GetDataAsString();

        try
        {
            byte[] pdfDocument = html2PdfGenerator.GeneratePdfDocument(html, propertiesHandler).GetAwaiter().GetResult();
            // var pdfDocument = html2PdfGenerator.GeneratePdfDocument(html, propertiesHandler);

            transformationContext.DataToBeSigned = new PadesDataToBeSigned(pdfDocument, Path.ChangeExtension(signersDocument.SignersDocumentFile.Name, "pdf"));
        }
        catch (Exception e)
        {
            throw new TransformationException($"Error transforming {signersDocument.SignersDocumentFile.Name} from HTML to PDF: {e.Message}", ErrorCode.SDK007, e);
        }
    }
}
