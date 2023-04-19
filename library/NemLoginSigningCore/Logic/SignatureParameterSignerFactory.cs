using NemLoginSigningCore.Logging;

namespace NemLoginSigningCore.Logic
{
    /// <summary>
    /// Factory class for creating ISignatureParameterSigner concrete implementation.
    /// </summary>
    public static class SignatureParameterSignerFactory
    {
        public static ISignatureParameterSigner Create()
        {
            return new SignatureParameterSigner(LoggerCreator.CreateLogger(nameof(SignatureParameterSigner)));
        }
    }
}