using System.Collections.Generic;
using Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation;
using PdfSharp.Pdf;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Pades
{
    public class PdfValidationResultV2Tests
    {
        [Fact]
        public void Constructor_WithValidData_SetsPropertiesCorrectly()
        {
            // Arrange
            var pdfObject = new KeyValuePair<string, PdfItem>("Key", new PdfDictionary());
            string pdfName = "Test.pdf";
            int objectNumber = 123;

            // Act
            var validationResult = new PdfValidationResultV2(pdfObject, pdfName, objectNumber);

            // Assert
            Assert.Equal(pdfObject, validationResult.PdfObject);
            Assert.Equal(pdfName, validationResult.PdfName);
            Assert.Equal(objectNumber, validationResult.ObjectNumber);
        }

        [Fact]
        public void Properties_WithValidData_AreImmutable()
        {
            // Arrange
            var pdfObject = new KeyValuePair<string, PdfItem>("Key", new PdfDictionary());
            string pdfName = "Test.pdf";
            int objectNumber = 123;
            var validationResult = new PdfValidationResultV2(pdfObject, pdfName, objectNumber);

            // Act & Assert
            Assert.Equal(pdfObject, validationResult.PdfObject);
            Assert.Equal(pdfName, validationResult.PdfName);
            Assert.Equal(objectNumber, validationResult.ObjectNumber);

            // Attempt to change properties
            pdfObject = new KeyValuePair<string, PdfItem>("NewKey", new PdfDictionary());
            pdfName = "NewTest.pdf";
            objectNumber = 456;

            // Assert properties remain unchanged
            Assert.Equal("Key", validationResult.PdfObject.Key);
            Assert.IsType<PdfDictionary>(validationResult.PdfObject.Value);
            Assert.Equal("Test.pdf", validationResult.PdfName);
            Assert.Equal(123, validationResult.ObjectNumber);
        }
    }
}