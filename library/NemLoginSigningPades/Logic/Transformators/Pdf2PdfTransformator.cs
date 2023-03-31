using System;
using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Format;
using NemLoginSigningCore.Logic;

namespace NemLoginSigningPades.Logic.Transformators
{
    /// <summary>
    /// Implementation of the ITransformator interface for handling transformations from
    /// PDF -> PDF which is only setting the DataToBeSigned when no transformation is needed.
    /// </summary>
    public class Pdf2PdfTransformator : ITransformator
    {
        public bool CanTransform(Transformation transformation)
        {
            return transformation.SignatureFormat == SignatureFormat.PAdES &&
                transformation.SdFormat == DocumentFormat.PDF;
        }

        public void Transform(TransformationContext ctx, ILogger logger)
        {
            SignersDocument sd = ctx.SignersDocument;
            DataToBeSigned dtbs = new PadesDataToBeSigned(sd.SignersDocumentFile.GetData(), sd.SignersDocumentFile.Name);
            ctx.DataToBeSigned = dtbs;
        }
    }
}
