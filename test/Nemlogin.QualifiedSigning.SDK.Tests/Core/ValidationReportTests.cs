using System;
using System.Collections.Generic;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Model;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Core
{
    public class ValidationReportTests
    {
        [Fact]
        public void Properties_Are_Initialized_Correctly()
        {
            // Arrange & Act
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
            var validation = new ValidationResult()
            {
                Signatures = new List<ValidationSignature>()
                {
                    signature
                }
            };
            var report = new ValidationReport()
            {
                Result = validation,
                EtsiReport = "Etsi"
            };

            // Assert
            Assert.NotNull(report.Result);
            Assert.IsType<ValidationResult>(report.Result);
            Assert.NotNull(report.EtsiReport);
        }
    }
}