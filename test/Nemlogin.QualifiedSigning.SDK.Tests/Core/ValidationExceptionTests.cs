using System;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Core
{
    public class ValidationExceptionTests
    {
        [Fact]
        public void Constructors_WithValidData_SetMessageCorrectly()
        {
            // Arrange
            string message = "Test message";

            // Act
            var exception = new ValidationException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void Constructors_WithValidData_SetInnerExceptionCorrectly()
        {
            // Arrange
            string message = "Test message";
            var innerException = new Exception("Inner exception message");

            // Act
            var exception = new ValidationException(message, innerException);

            // Assert
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void Constructors_WithValidData_SetErrorCodeCorrectly()
        {
            // Arrange
            string message = "Test message";
            ErrorCode errorCode = ErrorCode.SDK011;

            // Act
            var exception = new ValidationException(message, errorCode);

            // Assert
            Assert.Equal(errorCode, exception.ErrorCode);
        }
    }
}