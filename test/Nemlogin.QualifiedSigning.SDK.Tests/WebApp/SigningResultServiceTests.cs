using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Nemlogin.QualifiedSigning.Example.WebApp.Logic;
using Nemlogin.QualifiedSigning.SDK.Core.Configuration;
using Nemlogin.QualifiedSigning.SDK.Core.Services;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.WebApp
{
    public class SigningResultServiceTests
    {
        [Fact]
        public void Constructor_NullConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            var mockSigningValidationService = new Mock<ISigningValidationService>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SigningResultService(null, mockSigningValidationService.Object));
        }

        [Fact]
        public async Task ValidateSignedDocumentAsync_NullDocumentData_ThrowsArgumentNullException()
        {
            // Arrange
            var mockConfiguration = new Mock<IOptions<NemloginConfiguration>>();
            var mockSigningValidationService = new Mock<ISigningValidationService>();
            var service = new SigningResultService(mockConfiguration.Object, mockSigningValidationService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.ValidateSignedDocumentAsync("signedDocumentFilename", null));
        }

        [Fact]
        public void ParseError_NullParseError_ThrowsArgumentNullException()
        {
            // Arrange
            var mockConfiguration = new Mock<IOptions<NemloginConfiguration>>();
            var mockSigningValidationService = new Mock<ISigningValidationService>();
            var service = new SigningResultService(mockConfiguration.Object, mockSigningValidationService.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => service.ParseError(null));
        }

        [Fact]
        public void SignedDocumentFileName_ReturnsExpectedFormat()
        {
            // Arrange
            var mockConfiguration = new Mock<IOptions<NemloginConfiguration>>();
            var mockSigningValidationService = new Mock<ISigningValidationService>();
            var service = new SigningResultService(mockConfiguration.Object, mockSigningValidationService.Object);

            // Act
            var result = service.SignedDocumentFileName("example.docx", "pdf");

            // Assert
            Assert.Equal("example-signed.pdf", result);
        }

        // You can add more tests for other scenarios...
    }
}
