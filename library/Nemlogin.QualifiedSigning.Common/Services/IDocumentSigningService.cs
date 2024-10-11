using Nemlogin.QualifiedSigning.SDK.Core.DTO;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.Common.Services;

/// <summary>
/// Interface defining the SigningService for the Web Application
/// </summary>
public interface IDocumentSigningService
{
    public SigningPayloadDTO GenerateSigningPayload(SignatureFormat signatureFormat, SignatureKeys signatureKeys, string language, string referenceText, string filePath);
}