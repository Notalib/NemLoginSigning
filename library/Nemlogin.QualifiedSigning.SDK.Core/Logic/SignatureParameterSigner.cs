using System.Security.Cryptography.X509Certificates;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Utilities;

namespace Nemlogin.QualifiedSigning.SDK.Core.Logic;

/// <summary>
/// Signer class that is signing the SignatureParameters using Jose-JWT
/// </summary>
internal class SignatureParameterSigner : ISignatureParameterSigner
{
    public string Sign(SignatureParameters signatureParameters, SignatureKeys signatureKeys)
    {
        if (signatureParameters == null)
        {
            throw new ArgumentNullException(nameof(signatureParameters));
        }

        if (signatureKeys == null)
        {
            throw new ArgumentNullException(nameof(signatureKeys));
        }

        // The signing client should be passed the TU certificate 
        var certificate = signatureKeys.X509Certificate2;

        if (certificate == null)
        {
            throw new NullReferenceException("SignSdk - SignatureParameterSigner - Could not find Certificate");
        }
            
        // Headers should contain the Base64URLEncoded Certificate in a list
        Dictionary<string, object> headers = new Dictionary<string, object>() {
            { "x5c", new string[] { Convert.ToBase64String(certificate.Export(X509ContentType.Cert)) }},
            { "alg", "PS256" }
        };

        // Serialize json and deserialize to <string, object> for parsing to Jose.JWT
        var payload = ObjectSerializer.DeSerializeObject<Dictionary<string, object>>(ObjectSerializer.SerializeObject(signatureParameters));

        // Sign with privatekey PS256 using Jose.JWT
        return Jose.JWT.Encode(payload, certificate.GetRSAPrivateKey(), Jose.JwsAlgorithm.PS256, headers);
    }
}