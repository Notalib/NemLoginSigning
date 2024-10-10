using System.Security.Cryptography.X509Certificates;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.SDK.Core.Utilities;

/// <summary>
/// Utility class for loading signaturekeys from Windows Certificate store
/// </summary>
public class CertificateStoreSignatureKeysLoader
{
    private string _certificateFindByValue;
    private StoreLocation _storeLocation = StoreLocation.LocalMachine;
    private X509FindType _x509FindType = X509FindType.FindByThumbprint;
        
    public CertificateStoreSignatureKeysLoader FromWindowsStore(StoreLocation location = StoreLocation.LocalMachine)
    {
        _storeLocation = location;
        return this;
    }

    public CertificateStoreSignatureKeysLoader WithThumbprint(string thumbprint)
    {
        _certificateFindByValue = thumbprint;
        _x509FindType = X509FindType.FindByThumbprint;
            
        return this;
    }

    public SignatureKeys LoadSignatureKeys()
    {
        if (string.IsNullOrEmpty(_certificateFindByValue))
        {
            throw new ArgumentNullException("The Certificate FindBy Value must be defined");
        }
 
        var certificate = LoadCertificateFromWindowsStore();
        return new SignatureKeys(certificate, certificate.PrivateKey);
    }
        
    private X509Certificate2 LoadCertificateFromWindowsStore()
    {
        using var store = new X509Store(StoreName.My, _storeLocation);
        store.Open(OpenFlags.ReadOnly);
            
        var certificate = store.Certificates
            .Find(_x509FindType, _certificateFindByValue, true)
            .OfType<X509Certificate2>()
            .FirstOrDefault();

        if (certificate == null)
        {
            throw new InvalidOperationException($"Certificate with the following configuration was not found: {_storeLocation}, {_x509FindType}, {_certificateFindByValue}");
        }

        return certificate;
    }
}