using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.SDK.Core.Logic;

/// <summary>
/// Interface for implementation for signing the signature parameters.
/// </summary>
public interface ISignatureParameterSigner
{
    string Sign(SignatureParameters signatureParameters, SignatureKeys signatureKeys);
}