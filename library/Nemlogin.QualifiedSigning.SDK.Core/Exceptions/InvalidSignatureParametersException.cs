using Nemlogin.QualifiedSigning.SDK.Core.Enums;

namespace Nemlogin.QualifiedSigning.SDK.Core.Exceptions;

/// <summary>
/// Thrown if the {@link SignatureParameters} are invalid
/// </summary>
public class InvalidSignatureParametersException : NemLoginException
{
    public InvalidSignatureParametersException(string message) : base(message, ErrorCode.SDK002) { }

    public InvalidSignatureParametersException() { }

    public InvalidSignatureParametersException(string message, Exception innerException) : base(message, ErrorCode.SDK002, innerException) { }
}