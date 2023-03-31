namespace NemLoginSigningCore.Configuration
{
    /// <summary>
    /// Configuration class for loading the signaturekeys
    /// </summary>
    public class SignatureKeysConfiguration
    {
        /// <summary>
        /// The path to locate the P12 keystore file.
        /// </summary>
        public string KeystorePath { get; set; }

        /// <summary>
        /// Password for the keystore.
        /// </summary>
        public string KeyStorePassword { get; set; }

        /// <summary>
        /// Private key password for the keystore.
        /// </summary>
        public string PrivateKeyPassword { get; set; }
    }
}
