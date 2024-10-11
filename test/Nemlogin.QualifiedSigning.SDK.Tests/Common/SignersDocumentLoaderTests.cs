using System;
using System.Linq;
using Nemlogin.QualifiedSigning.Common.Services;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Common
{
    public class SignersDocumentLoaderTests
    {
        [Fact]
        public void GetFiles_ReturnsListOfSignersDocuments()
        {
            // Arrange
            var signersDocumentLoader = new SignersDocumentLoader();

            // Act
            var result = signersDocumentLoader.GetFiles();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(8, result.Count());
        }

        [Theory]
        [InlineData(".pdf", typeof(PdfSignersDocument))]
        [InlineData(".html", typeof(HtmlSignersDocument))]
        [InlineData(".txt", typeof(PlainTextSignersDocument))]
        [InlineData(".xml", typeof(XmlSignersDocument))]
        public void CreateSignersDocumentFromFile_ReturnsExpectedSignersDocument(string fileExtension, Type expectedDocumentType)
        {
            // Arrange
            var signersDocumentLoader = new SignersDocumentLoader();

            // Act
            var result = signersDocumentLoader.CreateSignersDocumentFromFile($"./Resources/SignersDocuments/heiberg2002011324{fileExtension}");

            // Assert
            Assert.NotNull(result);
            Assert.IsType(expectedDocumentType, result);
        }
    }
}
