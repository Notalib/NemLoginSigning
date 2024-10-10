using Nemlogin.QualifiedSigning.SDK.Core.Enums;

namespace Nemlogin.QualifiedSigning.SDK.Core.Validations;

/// <summary>
/// Factory for getting the correct validator
/// </summary>
public class ValidatorFactory: IValidationFactory
{
    private readonly IEnumerable<IValidator> _validators;

    public ValidatorFactory(IEnumerable<IValidator> validators)
    {
        _validators = validators;
    }

    public IValidator Create(DocumentFormat format)
    {
        var validator = _validators.FirstOrDefault(x => x.CanValidate(format));
        if(validator == null)
            throw new ArgumentException($"Could not find validator for documentformat: {format}.");

        return validator;
    }
}