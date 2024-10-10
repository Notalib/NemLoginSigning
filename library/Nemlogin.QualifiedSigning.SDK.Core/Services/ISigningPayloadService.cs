using Nemlogin.QualifiedSigning.SDK.Core.DTO;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.SDK.Core.Services;

/// <summary>
/// Interface for SigningPayloadService and entry for using the SignSdk library.
/// See 'SigningPayloadService' for documentation of how to use below methods.
/// </summary>
public interface ISigningPayloadService
{
    SigningPayload ProduceSigningPayload(TransformationContext ctx);

    SigningPayloadDTO ProduceSigningPayloadDTO(TransformationContext ctx);
}