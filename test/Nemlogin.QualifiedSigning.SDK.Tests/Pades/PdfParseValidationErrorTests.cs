using Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Pades
{
    public class PdfParseValidationErrorTests
    {
        [Fact]
        public void Constructor_WithParsedTrue_SetsParsedToTrue()
        {
            // Arrange & Act
            var error = new PdfParseValidationError(true);

            // Assert
            Assert.True(error.Parsed);
            Assert.Null(error.Error);
        }

        [Fact]
        public void Constructor_WithParsedFalseAndError_SetsParsedToFalseAndSetsError()
        {
            // Arrange
            var errorMessage = "Parsing failed";

            // Act
            var error = new PdfParseValidationError(errorMessage);

            // Assert
            Assert.False(error.Parsed);
            Assert.Equal(errorMessage, error.Error);
        }
    }
}