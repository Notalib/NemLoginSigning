using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Logic;

using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic;

public class PdfSourceAttacher : ISourceAttacher
{
    public void Attach(TransformationContext ctx)
    {
    }

    public bool CanAttach(Transformation transformation)
    {
        if (transformation == null)
        {
            throw new ArgumentNullException(nameof(transformation));
        }

        return (transformation.SdFormat == DocumentFormat.XML && transformation.SignatureFormat == SignatureFormat.PAdES);
    }
}