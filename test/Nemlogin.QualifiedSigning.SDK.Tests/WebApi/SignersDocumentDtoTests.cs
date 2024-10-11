using System;
using Nemlogin.QualifiedSigning.Example.WebApi.Model;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.WebApi
{
    public class SignersDocumentDtoTests
    {
        public class TestSignersDocumentDto : SignersDocumentDto
        {
            public override SignersDocument FromDto()
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void ToDto_ForTextDocument_ReturnsCorrectDtoInstance()
        {
            // Arrange
            var signersDocument = new PlainTextSignersDocument(new SignersDocumentFile(), new SignProperties());

            // Act
            var result = SignersDocumentDto.ToDto(signersDocument, false);

            // Assert
            Assert.IsType<PlainTextSignersDocumentDto>(result);
        }

        [Fact]
        public void ToDto_WhenSignersDocumentIsNull_Throws_Exception()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => SignersDocumentDto.ToDto(null, false));
        }
    }
}