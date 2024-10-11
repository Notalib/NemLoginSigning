using Nemlogin.QualifiedSigning.Example.WebApp.Models;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.WebApp
{
    public class DetailedSigningClientErrorTests
    {
        [Fact]
        public void ErrorCode_Should_SetAndGetCorrectly()
        {
            // Arrange
            var error = new DetailedSigningClientError();
            var expectedErrorCode = "123";

            // Act
            error.ErrorCode = expectedErrorCode;
            var actualErrorCode = error.ErrorCode;

            // Assert
            Assert.Equal(expectedErrorCode, actualErrorCode);
        }

        [Fact]
        public void ErrorMessage_Should_SetAndGetCorrectly()
        {
            // Arrange
            var error = new DetailedSigningClientError();
            var expectedErrorMessage = "An error occurred.";

            // Act
            error.ErrorMessage = expectedErrorMessage;
            var actualErrorMessage = error.ErrorMessage;

            // Assert
            Assert.Equal(expectedErrorMessage, actualErrorMessage);
        }
    }
}