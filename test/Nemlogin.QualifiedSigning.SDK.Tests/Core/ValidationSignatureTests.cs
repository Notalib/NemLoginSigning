using System;
using System.Collections.Generic;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Core
{
    public class ValidationSignatureTests
    {
        [Fact]
        public void Properties_WithValidData_HaveCorrectAttributes()
        {
            // Arrange
            var signature = new ValidationSignature();

            // Act
            var signatureFormatAttr = GetPropertyAttribute<JsonConverterAttribute>(nameof(signature.SignatureFormat));
            var signatureLevelAttr = GetPropertyAttribute<JsonConverterAttribute>(nameof(signature.SignatureLevel));
            var indicationAttr = GetPropertyAttribute<JsonConverterAttribute>(nameof(signature.Indication));
            var subIndicationAttr = GetPropertyAttribute<JsonConverterAttribute>(nameof(signature.SubIndication));

            // Assert
            Assert.NotNull(signatureFormatAttr);
            Assert.Equal(typeof(StringEnumConverter), signatureFormatAttr.ConverterType);
            Assert.NotNull(signatureLevelAttr);
            Assert.Equal(typeof(StringEnumConverter), signatureLevelAttr.ConverterType);
            Assert.NotNull(indicationAttr);
            Assert.Equal(typeof(StringEnumConverter), indicationAttr.ConverterType);
            Assert.NotNull(subIndicationAttr);
            Assert.Equal(typeof(StringEnumConverter), subIndicationAttr.ConverterType);
        }

        [Fact]
        public void CertificateChain_Is_Initialized()
        {
            // Arrange
            var signature = new ValidationSignature()
            {
                CertificateChain = new List<ValidationCertificate>()
                {
                    new ValidationCertificate()
                    {
                        SubjectDN = "Nemlog-In SDK",
                        NotAfter = DateTime.Now.AddDays(7),
                        NotBefore = DateTime.Now.AddDays(-7),
                        SerialNumber = Guid.NewGuid().ToString()
                    }
                }
            };

            // Assert
            Assert.NotNull(signature.CertificateChain);
            Assert.IsType<List<ValidationCertificate>>(signature.CertificateChain);
        }

        [Fact]
        public void Errors_Is_Initialized()
        {
            // Arrange
            var signature = new ValidationSignature()
            {
                Errors = new List<string>()
                {
                    "SDK001"
                }
            };

            // Assert
            Assert.NotNull(signature.Errors);
            Assert.IsType<List<string>>(signature.Errors);
        }

        [Fact]
        public void Warnings_Is_Initialized()
        {
            // Arrange
            var signature = new ValidationSignature()
            {
                Warnings = new List<string>()
                {
                    "WR001"
                }
            };

            // Assert
            Assert.NotNull(signature.Warnings);
            Assert.IsType<List<string>>(signature.Warnings);
        }

        [Fact]
        public void Infos_Is_Initialized()
        {
            // Arrange
            var signature = new ValidationSignature()
            {
                Infos = new List<string>()
                {
                    "IN001"
                }
            };

            // Assert
            Assert.NotNull(signature.Infos);
            Assert.IsType<List<string>>(signature.Infos);
        }
        
        [Fact]
        public void Initialized_WithValidDate_ReturnValidObject()
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

            // Assert
            Assert.Equal(SignatureFormat.PAdES, signature.SignatureFormat);
            Assert.Equal(SignatureLevel.ADES, signature.SignatureLevel);
            Assert.Equal(Indication.TOTAL_PASSED, signature.Indication);
            Assert.Equal(SubIndication.NO_POE, signature.SubIndication);
            Assert.NotNull(signature.SigningTime);
            Assert.NotNull(signature.SignedBy);
            Assert.NotNull(signature.Email);
        }

        private T GetPropertyAttribute<T>(string propertyName) where T : Attribute
        {
            return typeof(ValidationSignature)
                .GetProperty(propertyName)?
                .GetCustomAttributes(typeof(T), false)?
                .GetValue(0) as T;
        }
    }
}
