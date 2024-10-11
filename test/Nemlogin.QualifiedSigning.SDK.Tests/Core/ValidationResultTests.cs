using System;
using System.Collections.Generic;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Model;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Core
{
    public class ValidationResultTests
    {
        [Fact]
        public void Properties_AreInitializedCorrectly()
        {
            // Arrange
            var signature = new ValidationSignature()
            {
                SignatureFormat = SignatureFormat.PAdES,
                SignatureLevel = SignatureLevel.ADES,
                Indication = Indication.TOTAL_PASSED,
                SubIndication = SubIndication.NO_POE,
                SigningTime = DateTime.Now,
                SignedBy = "Qualifed Signing",
                Email = "qs@nets.eu"
            };
            var result = new ValidationResult()
            {
                Signatures = new List<ValidationSignature>()
                {
                    signature
                }
            };

            // Assert
            Assert.NotNull(result.Signatures);
            Assert.IsType<List<ValidationSignature>>(result.Signatures);
            Assert.Equal(0, result.SignaturesCount);
        }

    }
}