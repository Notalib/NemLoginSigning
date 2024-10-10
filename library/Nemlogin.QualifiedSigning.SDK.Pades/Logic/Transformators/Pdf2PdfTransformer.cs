using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Logic;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic.Transformators;

/// <summary>
/// Implementation of the ITransformator interface for handling transformations from
/// PDF -> PDF which is only setting the DataToBeSigned when no transformation is needed.
/// </summary>
public class Pdf2PdfTransformer : ITransformer
{
    public bool CanTransform(Transformation transformation)
    {
        return transformation.SignatureFormat == SignatureFormat.PAdES &&
               transformation.SdFormat == DocumentFormat.PDF;
    }

    public void Transform(TransformationContext ctx)
    {
        SignersDocument sd = ctx.SignersDocument;
        DataToBeSigned dtbs = new PadesDataToBeSigned(sd.SignersDocumentFile.GetData(), sd.SignersDocumentFile.Name);
        ctx.DataToBeSigned = dtbs;
    }
}