using System;
using Nemlogin.QualifiedSigning.SDK.Core.Model;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Core
{
    public class ValidationCertificateTests
    {
        [Fact]
        public void Properties_WithValidData_AreInitializedCorrectly()
        {
            // Arrange
            var certificate = new ValidationCertificate()
            {
                SubjectDN = "Nemlog-In SDK",
                NotAfter = DateTime.Now.AddDays(7),
                NotBefore = DateTime.Now.AddDays(-7),
                SerialNumber = Guid.NewGuid().ToString()
            };

            // Assert
            Assert.NotNull(certificate.SubjectDN);
            Assert.NotNull(certificate.SerialNumber);
            Assert.NotNull(certificate.NotBefore);
            Assert.NotNull(certificate.NotAfter);
            Assert.Null(certificate.Policies);
        }
    }
}