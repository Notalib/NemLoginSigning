using System;

using Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation;

using PdfSharp.Pdf;

using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Pades
{
    public class PdfFontValidatorV2Tests
    {
        [Fact]
        public void ValidateFonts_WhenEmptyPages_ReturnEmptyResults()
        {
            // Arrange
            var validator = new PdfFontValidatorV2();
            var document = new PdfDocument();
            document.AddPage();
            var emptyPages = document.Pages;

            // Act
            var results = validator.ValidateFonts(emptyPages);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void ScanForFonts_WhenNullPages_ThrowArgumentNullException()
        {
            // Arrange
            var validator = new PdfFontValidatorV2();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => validator.ScanForFonts(null));
        }

        [Theory]
        [InlineData("Helvetica")]
        [InlineData("Times-Roman")]
        [InlineData("Courier-Bold")]
        public void IsStandardFont_ForStandardFonts_ReturnTrue(string fontName)
        {
            // Act
            var isStandard = PdfFontValidatorV2.IsStandardFont(fontName);

            // Assert
            Assert.True(isStandard);
        }

        [Theory]
        [InlineData("Arial")]
        [InlineData("MyCustomFont")]
        public void IsStandardFont_ForNonStandardFonts_ReturnFalses(string fontName)
        {
            // Act
            var isStandard = PdfFontValidatorV2.IsStandardFont(fontName);

            // Assert
            Assert.False(isStandard);
        }
    }
}
