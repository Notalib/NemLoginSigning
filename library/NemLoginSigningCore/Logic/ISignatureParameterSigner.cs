using NemLoginSigningCore.Core;

namespace NemLoginSigningCore.Logic
{
    /// <summary>
    /// Interface for implementation for signing the signature parameters.
    /// </summary>
    public interface ISignatureParameterSigner
    {
        string Sign(SignatureParameters signatureParameters, SignatureKeys signatureKeys);
    }
}
