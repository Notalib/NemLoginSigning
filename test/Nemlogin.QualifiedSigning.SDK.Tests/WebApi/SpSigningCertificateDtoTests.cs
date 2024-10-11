using Nemlogin.QualifiedSigning.Example.WebApi.Model;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.WebApi
{
    public class SpSigningCertificateDtoTests
    {
        private class TestSpSigningCertificateDto : SpSigningCertificateDto
        {
        }

        [Fact]
        public void Keystore_WithValidData_SetAndGetCorrectly()
        {
            // Arrange
            var dto = new TestSpSigningCertificateDto();
            var expectedKeystore = "my-keystore";
            var expectedStorePassword = "Test123";
            var expectedPrivatePassword = "StrongPass123";

            // Act
            dto.Keystore = expectedKeystore;
            dto.KeyStorePassword = expectedStorePassword;
            dto.PrivateKeyPassword = expectedPrivatePassword;
            var actualStorePassword = dto.KeyStorePassword;
            var actualPrivatePassword = dto.PrivateKeyPassword;
            var actualKeystore = dto.Keystore;

            // Assert
            Assert.Equal(expectedKeystore, actualKeystore);
            Assert.Equal(expectedStorePassword, actualStorePassword);
            Assert.Equal(expectedPrivatePassword, actualPrivatePassword);
            
        }
        
    }
}