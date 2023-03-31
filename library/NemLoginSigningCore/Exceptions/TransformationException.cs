using System;

namespace NemLoginSigningCore.Exceptions
{
    /// <summary>
    /// Thrown when transforming a SD to an DTBS fails
    /// </summary>
    public class TransformationException : NemLoginException
    {
        public TransformationException()
        {
        }

        public TransformationException(string message)
            : base(message)
        {
        }

        public TransformationException(string message, ErrorCode errorCode)
            : base(message, errorCode)
        {
        }

        public TransformationException(string message, ErrorCode errorCode, Exception innerException)
            : base(message, errorCode, innerException)
        {
        }

        public TransformationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}