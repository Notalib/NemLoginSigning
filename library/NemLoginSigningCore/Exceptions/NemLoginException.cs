using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace NemLoginSigningCore.Exceptions
{
    /// <summary>
    /// Base NemLog-In exception class.
    /// </summary>
    public class NemLoginException : Exception
    {
        public ErrorCode ErrorCode { get; set; }

        public NemLoginException()
        {
        }

        public NemLoginException(string message)
            : base(message)
        {
        }

        public NemLoginException(string message, ErrorCode errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public NemLoginException(string message, ErrorCode errorCode, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        public NemLoginException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public string ErrorCodeDescription
        {
            get
            {
                return ErrorCode.GetType()
                            .GetMember(ErrorCode.ToString())
                            .First()?.GetCustomAttribute<DescriptionAttribute>()?.Description;
            }
        }
    }
}