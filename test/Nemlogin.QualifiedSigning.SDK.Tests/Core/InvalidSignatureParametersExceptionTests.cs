using System;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Core
{
    public class InvalidSignatureParametersExceptionTests
    {
        [Fact]
        public void Constructor_WithMessage_ShouldSetMessageAndErrorCode()
        {
            // Arrange
            var expectedMessage = "Invalid signature parameters";
            var expectedErrorCode = ErrorCode.SDK002;

            // Act
            var exception = new InvalidSignatureParametersException(expectedMessage);

            // Assert
            Assert.Equal(expectedMessage, exception.Message);
            Assert.Equal(expectedErrorCode, exception.ErrorCode);
        }

        [Fact]
        public void Constructor_WithInnerException_ShouldSetInnerException()
        {
            // Arrange
            var innerException = new Exception("Inner exception");

            // Act
            var exception = new InvalidSignatureParametersException("Invalid parameters", innerException);

            // Assert
            Assert.Same(innerException, exception.InnerException);
        }

        // Add more tests for other constructors and properties...
    }
}