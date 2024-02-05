using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Logging;

namespace NemLoginSigningCore.Utilities
{
    /// <summary>
    /// Utility class for loading signaturekeys from the keystore
    /// </summary>
    public class SignatureKeysLoader
    {
        private byte[] _keystore;

        protected string KeystorePath { get; set; }

        protected string KeyStoreType { get; set; }

        protected string KeyStorePassWord { get; set; }

        protected string PrivateKeyPassword { get; set; }

        public SignatureKeysLoader WithKeyStorePath(string keystorePath)
        {
            KeystorePath = keystorePath;
            return this;
        }

        public SignatureKeysLoader WithKeyStoreType(string keystoreType)
        {
            KeyStoreType = keystoreType;
            return this;
        }

        public SignatureKeysLoader WithKeyStore(byte[] keystore)
        {
            _keystore = keystore;
            return this;
        }

        public SignatureKeysLoader WithKeyStorePassword(string keyStorePassword)
        {
            KeyStorePassWord = keyStorePassword;
            return this;
        }

        public SignatureKeysLoader WithPrivateKeyPassword(string privateKeyPassword)
        {
            PrivateKeyPassword = privateKeyPassword;
            return this;
        }

        public SignatureKeys LoadSignatureKeys()
        {
            var logger = LoggerCreator.CreateLogger<SignatureKeysLoader>();

            logger.LogInformation("LoadSignatureKeys");

            if (string.IsNullOrEmpty(KeystorePath) && _keystore == null)
            {
                throw new ArgumentNullException("KeyStorePath or Keystore data must be defined");
            }

            if (PrivateKeyPassword == null)
            {
                throw new ArgumentNullException("The PrivateKeyPassword must be defined");
            }

            X509Certificate2Collection x509Certificate2Collection = ImportCertificateCollection(_keystore, KeystorePath);

            if (x509Certificate2Collection.OfType<X509Certificate2>().Where(c => c.HasPrivateKey).Count() != 1)
            {
                throw new CryptographicException("Certificate collection contains multiple certificates with a private key");
            }

            var certificate = x509Certificate2Collection.OfType<X509Certificate2>().Where(c => c.HasPrivateKey).Single();

            return new SignatureKeys(certificate, certificate.GetRSAPrivateKey());
        }

        private X509Certificate2Collection ImportCertificateCollection(byte[] keystore, string keystorePath)
        {
            X509Certificate2Collection x509Collection = new X509Certificate2Collection();
            if (_keystore != null)
            {
                x509Collection.Import(_keystore, PrivateKeyPassword, X509KeyStorageFlags.PersistKeySet);
            }
            else
            {
                x509Collection.Import(KeystorePath, PrivateKeyPassword, X509KeyStorageFlags.PersistKeySet);
            }

            return x509Collection;
        }
    }
}
