using System;

using Moq;

using Nemlogin.QualifiedSigning.Common.Services;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Common
{
    public interface IFile
    {
        bool Exists(string path);
    }
    
    public class TransformationPropertiesServiceTests
    {
        [Fact]
        public void GetTransformationProperties_NullSignersDocument_ThrowsArgumentNullException()
        {
            // Arrange
            var service = new TransformationPropertiesService();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => service.GetTransformationProperties(null, SignatureFormat.PAdES));
        }

        [Fact]
        public void GetTransformationProperties_NotPAdES_ReturnsEmptyProperties()
        {
            // Arrange
            var service = new TransformationPropertiesService();
            var mockSignersDocument = new Mock<SignersDocument>();

            // Act
            var result = service.GetTransformationProperties(mockSignersDocument.Object, SignatureFormat.XAdES);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetTransformationProperties_PAdESAndNotPDF_ReturnsEmptyProperties()
        {
            // Arrange
            var service = new TransformationPropertiesService();
            SignersDocument signersDocument = null;
            SignersDocumentFile signersDocumentFile = new SignersDocumentFile.SignersDocumentFileBuilder()
                .WithPath("./wwwroot/content/Files/old-pond.txt")
                .Build();
            
            signersDocument = new HtmlSignersDocument(signersDocumentFile);

            // Act
            var result = service.GetTransformationProperties(signersDocument, SignatureFormat.PAdES);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
        
        [Fact]
        public void GetTransformationProperties_NotPAdESAndPDF_ReturnsEmptyProperties()
        {
            // Arrange
            var service = new TransformationPropertiesService();
            SignersDocument signersDocument = null;
            SignersDocumentFile signersDocumentFile = new SignersDocumentFile.SignersDocumentFileBuilder()
                .WithPath("./Resources/SignersDocuments/heiberg2002011324.html")
                .Build();
            signersDocument = new PdfSignersDocument(signersDocumentFile);

            // Act
            var result = service.GetTransformationProperties(signersDocument, SignatureFormat.XAdES);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
