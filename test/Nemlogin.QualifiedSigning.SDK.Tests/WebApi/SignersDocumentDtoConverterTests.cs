using System;
using Nemlogin.QualifiedSigning.Example.WebApi.Model;
using Newtonsoft.Json;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.WebApi
{
    public class SignersDocumentDtoConverterTests
    {
        [Theory]
        [InlineData("{\"format\":\"PDF\",\"otherProperty\":\"value\"}", typeof(PdfSignersDocumentDto))]
        [InlineData("{\"format\":\"HTML\",\"otherProperty\":\"value\"}", typeof(HtmlSignersDocumentDto))]
        [InlineData("{\"format\":\"XML\",\"otherProperty\":\"value\"}", typeof(XmlSignersDocumentDto))]
        [InlineData("{\"format\":\"TEXT\",\"otherProperty\":\"value\"}", typeof(PlainTextSignersDocumentDto))]
        public void ReadJson_WithValidJson_ReturnsCorrectType(string json, Type expectedType)
        {
            // Arrange
            var converter = new SignersDocumentDtoConverter();
            var reader = new JsonTextReader(new System.IO.StringReader(json));
            
            // Act
            var result = converter.ReadJson(reader, typeof(SignersDocumentDto), null, JsonSerializer.CreateDefault());

            // Assert
            Assert.NotNull(result);
            Assert.IsType(expectedType, result);
        }

        [Fact]
        public void ReadJson_WhenFormatIsNotSupported_ThrowsException()
        {
            // Arrange
            var converter = new SignersDocumentDtoConverter();
            var reader = new JsonTextReader(new System.IO.StringReader("{\"format\":\"INVALID\"}"));
            
            // Act & Assert
            Assert.Throws<Exception>(() => converter.ReadJson(reader, typeof(SignersDocumentDto), null, JsonSerializer.CreateDefault()));
        }

        [Fact]
        public void CanConvert_ForSignersDocumentDto_ReturnsTrue()
        {
            // Arrange
            var converter = new SignersDocumentDtoConverter();

            // Act
            var result = converter.CanConvert(typeof(SignersDocumentDto));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanConvert_ForOtherTypes_ReturnsFalse()
        {
            // Arrange
            var converter = new SignersDocumentDtoConverter();

            // Act
            var result = converter.CanConvert(typeof(object));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanWrite_ReturnsFalse()
        {
            // Arrange
            var converter = new SignersDocumentDtoConverter();

            // Act
            var result = converter.CanWrite;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WriteJson_WithNullValues_ThrowsNotSupportedException()
        {
            // Arrange
            var converter = new SignersDocumentDtoConverter();

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => converter.WriteJson(null, null, null));
        }
    }
}
