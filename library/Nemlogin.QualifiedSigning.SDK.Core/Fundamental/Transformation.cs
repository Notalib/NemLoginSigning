using Nemlogin.QualifiedSigning.SDK.Core.Enums;

namespace Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

/// <summary>
/// Defines a Transformation from one document format to another.
/// Concrete types of transformations are hard-coded in ValidTransformation class.
/// </summary>
public class Transformation
{
    public DocumentFormat SdFormat { get; private set; }

    public SignatureFormat SignatureFormat { get; private set; }

    public ViewFormat ViewFormat { get; private set; }

    public Transformation(DocumentFormat sdFormat, SignatureFormat signatureFormat, ViewFormat viewFormat)
    {
        SdFormat = sdFormat;
        SignatureFormat = signatureFormat;
        ViewFormat = viewFormat;
    }
}