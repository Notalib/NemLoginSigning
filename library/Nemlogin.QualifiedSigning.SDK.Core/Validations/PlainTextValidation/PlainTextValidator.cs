using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.SDK.Core.Validations.PlainTextValidation;

/// <summary>
/// Validation of plain text. No validation is needed.
/// </summary>
public class PlainTextValidator : IValidator
{
    public void Validate(TransformationContext ctx)
    {
    }

    public bool CanValidate(DocumentFormat format)
    {
        return format == DocumentFormat.TEXT;
    }
}