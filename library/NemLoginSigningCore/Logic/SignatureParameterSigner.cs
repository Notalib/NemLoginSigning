using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;

using static NemLoginSigningCore.Utilities.ObjectSerializer;

namespace NemLoginSigningCore.Logic
{
    /// <summary>
    /// Signer class that is signing the SignatureParameters using Jose-JWT
    /// </summary>
    internal class SignatureParameterSigner : ISignatureParameterSigner
    {
        private readonly ILogger _logger;

        public SignatureParameterSigner(ILogger logger)
        {
            _logger = logger;
        }

        public string Sign(SignatureParameters signatureParameters, SignatureKeys signatureKeys)
        {
            ArgumentNullException.ThrowIfNull(signatureParameters);

            ArgumentNullException.ThrowIfNull(signatureKeys);

            // The signing client should be passed the TU certificate
            _logger.LogInformation("SignSdk - SignatureParameterSigner - GetFirstCertificateInChain");

            X509Certificate2 certificate = signatureKeys.X509Certificate2;

            if (certificate == null)
            {
                _logger.LogInformation("SignSdk - SignatureParameterSigner - Could not find Certificate");
                throw new ArgumentException($"Could not find Voces certificate in {signatureKeys}");
            }

            // Headers should contain the Base64URLEncoded Certificate in a list
            Dictionary<string, object> headers = new()
            {
                { "x5c", new List<string>() { Convert.ToBase64String(certificate.Export(X509ContentType.Cert)) } },
                { "alg", "PS256" }
            };

            // Serialize json and deserialize to <string, object> for parsing to Jose.JWT
            var payload = DeSerializeObject<Dictionary<string, object>>(SerializeObject(signatureParameters));

            // Sign with privatekey PS256 using Jose.JWT
            _logger.LogInformation("SignSdk - SignatureParameterSigner - JWT Encode using Jose.JWT");

            return Jose.JWT.Encode(payload, certificate.GetRSAPrivateKey(), Jose.JwsAlgorithm.PS256, headers);
        }
    }
}
