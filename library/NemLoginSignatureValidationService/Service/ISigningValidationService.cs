using System.Threading.Tasks;
using NemLoginSignatureValidationService.Model;

namespace NemLoginSignatureValidationService.Service
{
    /// <summary>
    /// Interface for the SigningValidationService
    /// Simple implementation of a service for validating a signed document
    /// by calling the public NemLog-In Signature Validation API.
    /// </summary>
    public interface ISigningValidationService
    {
        Task<ValidationReport> Validate(SignatureValidationContext ctx);
    }
}