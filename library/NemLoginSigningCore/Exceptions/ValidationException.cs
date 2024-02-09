using System;

namespace NemLoginSigningCore.Exceptions
{
    /// <summary>
    /// Thrown when validating SD fails
    /// </summary>
    public class ValidationException : NemLoginException
    {
        public ValidationException()
        {
        }

        public ValidationException(string message)
            : base(message)
        {
        }

        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ValidationException(string message, ErrorCode errorCode)
            : base(message, errorCode)
        {
        }

        public ValidationException(string message, ErrorCode errorCode, Exception innerException)
            : base(message, errorCode, innerException)
        {
        }
    }
}
