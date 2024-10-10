using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

using NemLoginSigningDTO.Signing;

namespace NemLoginSigningWebApp.Logic;

/// <summary>
/// Interface defining the SigningService for the Web Application
/// </summary>
public interface IDocumentSigningService
{
    public SigningPayloadDTO GenerateSigningPayload(SignersDocument signersDocument, SignatureParameters signatureParameters, SignatureFormat signatureFormat, SignatureKeys signatureKeys);
}
