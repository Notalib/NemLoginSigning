using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

/// <summary>
/// SignatureKeys encapsulated the Service Providers certificate chain
/// used for the signing.
/// </summary>
public class SignatureKeys
{
    public AsymmetricAlgorithm PrivateKey { get; private set; } 
        
    public X509Certificate2 X509Certificate2 { get; private set; }

    public SignatureKeys(X509Certificate2 certificate, AsymmetricAlgorithm privateKey)
    {
        X509Certificate2 = certificate;
        PrivateKey = privateKey;
    }
}