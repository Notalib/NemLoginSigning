using System.Diagnostics;
using System.Text;

using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Logic;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic.Transformators;

/// <summary>
/// Baseclass for PDF transformations which are inherited from the specific transformation classes
/// </summary>

public abstract class PdfFormatTransformationService : ITransformer
{
    public abstract bool CanTransform(Transformation transformation);

    protected abstract string GenerateHtml(TransformationContext ctx);

    public void Transform(TransformationContext transformationContext)
    {
        Stopwatch sw = Stopwatch.StartNew();

        SignersDocument signersDocument = transformationContext.SignersDocument;

        // Step 1: Generate HTML by transforming the XML using the included XSLT
        string html = GenerateHtml(transformationContext);

        // Step 2: Generate PDF from the HTML
        try
        {
            string fileName = transformationContext.SignersDocument.SignersDocumentFile.Name;
            byte[] pdf = GeneratePDF(html, fileName, transformationContext.TransformationProperties);

            DataToBeSigned dtbs = new PadesDataToBeSigned(pdf, Path.ChangeExtension(fileName, "pdf"));

            transformationContext.DataToBeSigned = dtbs;
        }
        catch (Exception e)
        {
            string logMessage = $"Error transforming from {signersDocument.SignersDocumentFile.Name} from {signersDocument.DocumentFormat} to PDF: {e.Message}";
            throw new TransformationException(logMessage, ErrorCode.SDK007, e);
        }
    }

    public byte[] GeneratePDF(string html, string name, TransformationProperties transformationProperties)
    {
        Transformation transformation = ValidTransformation.GetTransformation(DocumentFormat.HTML, SignatureFormat.PAdES);

        ITransformer transformator = TransformatorFactory.Create(transformation);

        SignersDocumentFile signersDocumentFile = new SignersDocumentFile.SignersDocumentFileBuilder()
            .WithData(Encoding.UTF8.GetBytes(html))
            .WithName(name)
            .Build();

        SignatureParameters signatureParameters = new SignatureParameters.SignatureParametersBuilder()
            .WithValidTransformation(transformation)
            .Build();

        TransformationContext ctx = new TransformationContext(
            new HtmlSignersDocument(signersDocumentFile),
            null,
            signatureParameters,
            transformationProperties);

        transformator.Transform(ctx);

        return ctx.DataToBeSigned.GetData();
    }
}
