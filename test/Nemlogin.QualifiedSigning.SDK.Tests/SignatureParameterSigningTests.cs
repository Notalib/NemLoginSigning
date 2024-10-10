using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Logic;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests
{
    /// <summary>
    /// Test and validate the content of the generated signing payload.
    /// </summary>
    public class SignatureParameterSigningTests : SigningTestBase
    {
        [Fact]
        public void SigningOfSigningParameters()
        {
            // Arrange 
            SignatureParameters signatureParameters = new SignatureParameters.SignatureParametersBuilder()
                .WithFlowType(FlowType.ServiceProvider)
                .WithSignersDocumentFormat(DocumentFormat.PDF)
                .WithSignatureFormat(SignatureFormat.PAdES)
                .WithDtbsDigest("XXX")
                .WithDtbsSignedInfo("XXX")
                .WithReferenceText("signering af xxx")
                .WithMinAge(18)
                .WithAnonymizeSigner(true)
                .Build();

            // Act
            string signedParameters = SignatureParameterSignerFactory.Create().Sign(signatureParameters, SignatureKeys);

            // Assert
            var headers = Jose.JWT.Headers(signedParameters);
            var payload = Jose.JWT.Payload(signedParameters, true);
            
            // Validate correct algorithm
            Assert.Equal("PS256", headers["alg"]);

            // Validate correct certificate in the header
            var header = (IEnumerable<object>)headers["x5c"];
            Assert.NotNull(header);
            
            var certificate = Convert.FromBase64String(header.First().ToString());
            using (X509Certificate2 x509Certificate2 = new X509Certificate2(certificate))
            {
                Assert.Equal(SignatureKeys.X509Certificate2, x509Certificate2);

                // Verify the signature - if signature is not valid Decode will throw exception
                var exception = Record.Exception(() => { Jose.JWT.Decode(signedParameters, x509Certificate2.GetRSAPublicKey(), Jose.JwsAlgorithm.PS256); });
                Assert.Null(exception);
                
            }
        }

        [Fact]
        public void SignatureParametersForBrokerFlowShouldValidateCorrect()
        {
            SignatureParameters signatureParameters = new SignatureParameters.SignatureParametersBuilder()
                .WithFlowType(FlowType.ServiceProvider)
                .WithSignersDocumentFormat(DocumentFormat.XML)
                .WithSignatureFormat(SignatureFormat.XAdES)
                .WithPreferredLanguage(Language.da)
                .WithReferenceText("Signing of document")
                .WithSsnPersistenceLevel(SsnPersistenceLevel.Session)
                .WithPreferredLanguage(Language.en)
                .WithSignerSubjectNameID("SignersSubjectNameID Test")
                .WithAnonymizeSigner(true)
                .WithAcceptedCertificatePolicies(AcceptedCertificatePolicies.Employee | AcceptedCertificatePolicies.Organization | AcceptedCertificatePolicies.Person)
                .WithMinAge(18)
                .WithDtbsSignedInfo("SignedInfoForValidationTest")
                .WithDtbsDigest("DtbsDigestForValidationTest")
                .WithEntityID("https://signsdk-demo.nemlog-in.dk")
                .Build();

            signatureParameters.Validate();

        }
    }
}