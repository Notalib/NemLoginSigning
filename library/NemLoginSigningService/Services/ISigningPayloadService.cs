using NemLoginSigningCore.Core;
using NemLoginSigningDTO.Signing;

namespace NemLoginSigningService.Services
{
    /// <summary>
    /// Interface for SigningPayloadService and entry for using the SignSdk library.
    /// See 'SigningPayloadService' for documentation of how to use below methods.
    /// </summary>
    public interface ISigningPayloadService
    {
        SigningPayload ProduceSigningPayload(TransformationContext context);

        SigningPayloadDTO ProduceSigningPayloadDTO(TransformationContext context);
    }
}