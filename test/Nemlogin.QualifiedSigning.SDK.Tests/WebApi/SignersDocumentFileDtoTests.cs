using System;
using Nemlogin.QualifiedSigning.Example.WebApi.Model;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.WebApi
{
    public class SignersDocumentFileDtoTests
    {
        [Fact]
        public void Constructor_WhenSignersDocumentFileIsNull_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SignersDocumentFileDto(null, false));
        }

        [Fact]
        public void FromDto_WithValidFile_ReturnsCorrectSignersDocumentFileInstance()
        {
            // Arrange
            SignersDocumentFile signersDocumentFile = new SignersDocumentFile.SignersDocumentFileBuilder()
                .WithPath("./Resources/SignersDocuments/heiberg2002011324.pdf")
                .Build();

            var signersDocumentFileDto = new SignersDocumentFileDto(signersDocumentFile, true);

            // Act
            var result = signersDocumentFileDto.FromDto();

            // Assert
            Assert.Equal(signersDocumentFileDto.Uri, result.Uri);
        }

        // Add more tests as needed to cover other scenarios
    }
}