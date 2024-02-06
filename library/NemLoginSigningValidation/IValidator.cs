using NemLoginSigningCore.Core;

namespace NemLoginSigningValidation
{
    /// <summary>
    /// Interface for defining Validator contract for Validator implemetation
    /// </summary>
    public interface IValidator
    {
        void Validate(TransformationContext context);
    }
}
