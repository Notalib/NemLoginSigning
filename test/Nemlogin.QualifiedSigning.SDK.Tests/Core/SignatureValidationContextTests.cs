using System;
using System.IO;
using Nemlogin.QualifiedSigning.SDK.Core.Model;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Core
{
    public class SignatureValidationContextTests
    {
        [Fact]
        public void Constructor_CopyFromOtherContext_InitializesProperties()
        {
            // Arrange
            var originalContext = new SignatureValidationContext.SignatureValidationContextBuilder()
                .WithValidationServiceUrl("https://example.com")
                .WithTimeout(5000)
                .WithDocumentName("test.txt")
                .WithDocumentData(new byte[] { 0x01, 0x02, 0x03 })
                .Build();

            // Act
            var copiedContext = new SignatureValidationContext(originalContext);

            // Assert
            Assert.Equal(originalContext.ValidationServiceUrl, copiedContext.ValidationServiceUrl);
            Assert.Equal(originalContext.Timeout, copiedContext.Timeout);
            Assert.Equal(originalContext.DocumentName, copiedContext.DocumentName);
            Assert.Equal(originalContext.GetDocumentData(), copiedContext.GetDocumentData());
        }

        [Fact]
        public void Builder_WithValidValues_BuildsContext()
        {
            // Act
            var context = new SignatureValidationContext.SignatureValidationContextBuilder()
                .WithValidationServiceUrl("https://example.com")
                .WithTimeout(5000)
                .WithDocumentName("test.txt")
                .WithDocumentData(new byte[] { 0x01, 0x02, 0x03 })
                .Build();

            // Assert
            Assert.NotNull(context);
            Assert.Equal("https://example.com", context.ValidationServiceUrl);
            Assert.Equal(5000, context.Timeout);
            Assert.Equal("test.txt", context.DocumentName);
            Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, context.GetDocumentData());
        }

        [Fact]
        public void Builder_WithDocumentPath_LoadsFile()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt");
            File.WriteAllBytes(filePath, new byte[] { 0x01, 0x02, 0x03 });

            // Act
            var context = new SignatureValidationContext.SignatureValidationContextBuilder()
                .WithValidationServiceUrl("https://example.com")
                .WithTimeout(5000)
                .WithDocumentPath(filePath)
                .Build();

            // Assert
            Assert.NotNull(context);
            Assert.Equal("https://example.com", context.ValidationServiceUrl);
            Assert.Equal(5000, context.Timeout);
            Assert.Equal("test.txt", context.DocumentName);
            Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, context.GetDocumentData());

            // Clean up
            File.Delete(filePath);
        }

        [Fact]
        public void Builder_WithInvalidValues_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SignatureValidationContext.SignatureValidationContextBuilder().Build());
            Assert.Throws<ArgumentNullException>(() => new SignatureValidationContext.SignatureValidationContextBuilder().WithValidationServiceUrl("").Build());
            Assert.Throws<ArgumentNullException>(() => new SignatureValidationContext.SignatureValidationContextBuilder().WithDocumentName("").Build());
            Assert.Throws<ArgumentNullException>(() => new SignatureValidationContext.SignatureValidationContextBuilder().WithDocumentData(null).Build());
        }
    }
}
