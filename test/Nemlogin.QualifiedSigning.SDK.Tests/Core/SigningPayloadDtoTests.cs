using System;
using Nemlogin.QualifiedSigning.SDK.Core;
using Nemlogin.QualifiedSigning.SDK.Core.DTO;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Core
{
    public class SigningPayloadDtoTests
    {
        [Fact]
        public void Constructor_WithValidSigningPayload_InitializesProperties()
        {
            // Arrange
            var testData = new byte[] { 0x01, 0x02, 0x03 };
            var signingPayload = new SigningPayload("SampleParameters", new DataToBeSigned(SignatureFormat.PAdES, testData, "test.pdf"));

            // Act
            var dto = new SigningPayloadDTO(signingPayload);

            // Assert
            Assert.Equal(signingPayload.SignatureParameters, dto.SignatureParameters);
            Assert.Equal(Convert.ToBase64String(testData), dto.Dtbs);
        }

        [Fact]
        public void Constructor_WithNullSigningPayload_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SigningPayloadDTO(null));
        }
    }
}