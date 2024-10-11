using System;
using System.IO;
using Moq;
using Nemlogin.QualifiedSigning.Common.Services;
using Nemlogin.QualifiedSigning.SDK.Core;
using Nemlogin.QualifiedSigning.SDK.Core.Configuration;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Services;
using Newtonsoft.Json;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Common
{
    public class DocumentSigningServiceTests: SigningTestBase
    {
        [Fact]
        public void GenerateSigningPayload_IfNemloginConfigurationIsValid_ReturnsSigningPayloadDTO()
        {
            // Arrange
            var signingPayloadServiceMock = new Mock<ISigningPayloadService>();
            var signersDocumentLoaderMock = new Mock<ISignersDocumentLoader>();
            var transformationPropertiesServiceMock = new Mock<ITransformationPropertiesService>();
            
            var json = File.ReadAllText("appsettings.json");
            var nemloginConfiguration = JsonConvert.DeserializeObject<NemloginConfiguration>(json);
            
            SignersDocument signersDocument = null;
            SignersDocumentFile signersDocumentFile = new SignersDocumentFile.SignersDocumentFileBuilder()
                .WithPath("./Resources/SignersDocuments/heiberg2002011324.pdf")
                .Build();
            signersDocument = new PdfSignersDocument(signersDocumentFile);

            var service = new DocumentSigningService(
                signingPayloadServiceMock.Object,
                signersDocumentLoaderMock.Object,
                transformationPropertiesServiceMock.Object,
                nemloginConfiguration);

            var signatureFormat = SignatureFormat.PAdES;
            var language = "en";
            var referenceText = "Test reference";
            var filePath = signersDocumentFile.Path;

            // Set up mocks or create instances as needed
            signersDocumentLoaderMock.Setup(loader => loader.CreateSignersDocumentFromFile(It.IsAny<string>())).Returns(signersDocument);
            var testData = new byte[] { 0x01, 0x02, 0x03 };
            var signingPayload = new SigningPayload("SampleParameters", new DataToBeSigned(SignatureFormat.PAdES, testData, "test.pdf"));
            signingPayloadServiceMock.Setup(s => s.ProduceSigningPayload(It.IsAny<TransformationContext>())).Returns(signingPayload);

            // Act
            var result = service.GenerateSigningPayload(signatureFormat, SignatureKeys, language, referenceText, filePath);

            // Assert
            Assert.NotNull(result);
            // Add more assertions as needed based on the expected behavior of the GenerateSigningPayload method
        }
        
                [Fact]
        public void GenerateSigningPayload_IfNemloginConfigurationIsNull_ArgumentNullException()
        {
            // Arrange
            var signingPayloadServiceMock = new Mock<ISigningPayloadService>();
            var signersDocumentLoaderMock = new Mock<ISignersDocumentLoader>();
            var transformationPropertiesServiceMock = new Mock<ITransformationPropertiesService>();
            
            var json = string.Empty;
            var nemloginConfiguration = JsonConvert.DeserializeObject<NemloginConfiguration>(json);

            // Assert
            Assert.Throws<ArgumentNullException>(() => new DocumentSigningService(
                signingPayloadServiceMock.Object,
                signersDocumentLoaderMock.Object,
                transformationPropertiesServiceMock.Object,
                nemloginConfiguration));
        }
    }
    
    
}
