using Nemlogin.QualifiedSigning.SDK.Pades;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Pades
{
    public class EncryptionAlgorithmsTests
    {
        [Theory]
        [InlineData("1.2.840.113549.1.1.1", "RSA")]
        [InlineData("1.2.840.10040.4.1", "DSA")]
        [InlineData("1.3.14.3.2.29", "RSA")]
        [InlineData("1.2.643.2.2.19", "ECGOST3410")]
        [InlineData("unknown-oid", "unknown-oid")] // Test for an unknown OID
        public void GetAlgorithm_Should_ReturnExpectedResult(string oid, string expectedAlgorithm)
        {
            // Arrange
            var algorithm = EncryptionAlgorithms.GetAlgorithm(oid);

            // Assert
            Assert.Equal(expectedAlgorithm, algorithm);
        }
    }
}