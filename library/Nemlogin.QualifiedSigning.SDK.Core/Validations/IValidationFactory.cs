using Nemlogin.QualifiedSigning.SDK.Core.Enums;

namespace Nemlogin.QualifiedSigning.SDK.Core.Validations;

public interface IValidationFactory
{
    IValidator Create(DocumentFormat format);
}