using System;
using System.Text;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.DTO;

namespace NemLoginSigningService.Services
{
    /// <summary>
    /// Interface for SigningPayloadService and entry for using the SignSdk library.
    /// See 'SigningPayloadService' for documentation of how to use below methods.
    /// </summary>
    public interface ISigningPayloadService
    {
        SigningPayload ProduceSigningPayload(TransformationContext ctx);

        SigningPayloadDTO ProduceSigningPayloadDTO(TransformationContext ctx);
    }
}