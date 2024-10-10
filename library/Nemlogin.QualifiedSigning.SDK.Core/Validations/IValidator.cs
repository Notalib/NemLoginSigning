using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.SDK.Core.Validations;

/// <summary>
/// Interface for defining Validator contract for Validator implemetation
/// </summary>
public interface IValidator
{
    void Validate(TransformationContext ctx);
    bool CanValidate(DocumentFormat format);
}