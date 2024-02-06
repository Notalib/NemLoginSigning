using System;
using System.Text;

using Microsoft.Extensions.Logging.Abstractions;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Logging;
using NemLoginSigningCore.Utilities;

namespace NemloginSigningTest
{
    public abstract class SigningTestBase
    {
        protected SigningTestBase()
        {
            LoggerCreator.LoggerFactory = new NullLoggerFactory();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public string EntityID
        {
            get
            {
                if (TestHelper.GetConfiguration(TestConstants.ConfigurationNemlogin).SignatureKeysConfiguration is null)
                {
                    throw new InvalidOperationException("No signaturekeys configuration");
                }

                string entityID = TestHelper.GetConfiguration(TestConstants.ConfigurationNemlogin).EntityID;

                if (entityID == null)
                {
                    throw new InvalidOperationException("EntityID is not configured");
                }

                return entityID;
            }
        }

        private SignatureKeys _signatureKeys;

        public SignatureKeys SignatureKeys
        {
            get
            {
                if (_signatureKeys == null)
                {
                    _signatureKeys = GetSignatureKeys();
                }

                return _signatureKeys;
            }
        }

        private SignatureKeys GetSignatureKeys()
        {
            var configuration = TestHelper.GetConfiguration(TestConstants.ConfigurationNemlogin);

            if (configuration.SignatureKeysConfiguration is null)
            {
                throw new InvalidOperationException("No signaturekeys configuration");
            }

            var signatureKeysConfig = configuration.SignatureKeysConfiguration;

            return new SignatureKeysLoader()
               .WithKeyStorePath(signatureKeysConfig.KeystorePath)
               .WithKeyStorePassword(signatureKeysConfig.KeyStorePassword)
               .WithPrivateKeyPassword(signatureKeysConfig.PrivateKeyPassword)
               .LoadSignatureKeys();
        }
    }
}