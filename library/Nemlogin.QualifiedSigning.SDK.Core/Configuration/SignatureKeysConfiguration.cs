using System.Security.Cryptography.X509Certificates;

namespace Nemlogin.QualifiedSigning.SDK.Core.Configuration;

public class SignatureKeysConfiguration
{
    /// <summary>
    /// A Broker-specific entity ID provisioned using the NemLog-In Administration Component
    /// </summary>
    public string EntityId { get; set; }

    /// <summary>
    /// The path to locate the P12 keystore file.
    /// </summary>
    public string KeystorePath { get; set; }

    /// <summary>
    /// Password for the keystore.
    /// </summary>
    public string KeystorePassword { get; set; }

    /// <summary>
    /// Private key password for the keystore.
    /// </summary>
    public string PrivateKeyPassword { get; set; }
        
    /// <summary>
    /// The certificate thumbprint for loading certificate from Windows store.
    /// </summary>
    public string CertificateThumbprint { get; set; }

    /// <summary>
    /// The CertificateStore location
    /// </summary>
    public StoreLocation CertificateStoreLocation { get; set; } = StoreLocation.LocalMachine;

    public override string ToString() {
        return "SignatureKeysConfiguration {"
            + " EntityId = " + EntityId 
            + " KeystorePath = " + KeystorePath
            + " CertificateThumbprint = " + CertificateThumbprint
            + " CertificateStoreLocation = " + CertificateStoreLocation
            + " }";
    }  

}