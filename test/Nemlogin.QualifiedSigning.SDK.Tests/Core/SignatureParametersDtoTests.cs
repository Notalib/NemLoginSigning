using Nemlogin.QualifiedSigning.Example.WebApi.Model;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Core
{
    public class SignatureParametersDtoTests
    {
        [Fact]
        public void Constructor_WithoutAnyParameter_SetsDefaultsPropertiesCorrectly()
        {
            // Arrange & Act
            var signatureParametersDto = new SignatureParametersDto();

            // Assert
            Assert.NotNull(signatureParametersDto);
            Assert.Equal(1, signatureParametersDto.Version);
            Assert.Equal("SHA-256", signatureParametersDto.DtbsDigestAlgorithm);
        }

        [Fact]
        public void Constructor_WithValidData_CopiesPropertiesCorrectly()
        {
            // Arrange
            var signatureParameters = new SignatureParameters
            {
                EntityID = "TestValue"
            };

            // Act
            var signatureParametersDto = new SignatureParametersDto(signatureParameters);

            // Assert
            Assert.NotNull(signatureParametersDto);
            Assert.Equal(signatureParameters.EntityID, signatureParametersDto.EntityID); // Assuming SomeProperty is a property in SignatureParameters
            // Add more assertions for other properties if needed
        }

        // Add more tests as needed to cover other scenarios or properties
    }
}