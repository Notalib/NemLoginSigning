using Microsoft.Extensions.Logging.Abstractions;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Logging;
using NemLoginSigningCore.Utilities;
using System;
using System.Linq;
using System.Text;

namespace NemloginSigningTest
{
    public abstract class SigningTestBase
    {
        public SigningTestBase()
        {
            LoggerCreator.LoggerFactory = new NullLoggerFactory();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public string EntityID
        {
            get
            {
                if (!TestHelper.GetConfiguration(TestConstants.ConfigurationNemlogin).SignatureKeysConfiguration.Any())
                {
                    throw new InvalidOperationException("No signaturekeys configuration");
                }
                        
                var entityID = TestHelper.GetConfiguration(TestConstants.ConfigurationNemlogin).SignatureKeysConfiguration.First().EntityID;

                if (entityID == null)
                {
                    throw new InvalidOperationException("EntityID is not configured");
                }

                return entityID;
            }
        }

        private SignatureKeys signatureKeys;
        public SignatureKeys SignatureKeys 
        { 
            get
            {
                if (signatureKeys == null)
                {
                    signatureKeys = GetSignatureKeys();
                }

                return signatureKeys;
            }
        }

        private SignatureKeys GetSignatureKeys()
        {
            var configuration = TestHelper.GetConfiguration(TestConstants.ConfigurationNemlogin);

            if (!configuration.SignatureKeysConfiguration.Any())
            {
                throw new InvalidOperationException("No signaturekeys configuration");
            }

            var signatureKeysConfig = configuration.SignatureKeysConfiguration.First();

            return new SignatureKeysLoader()
               .WithKeyStorePath(signatureKeysConfig.KeystorePath)
               .WithKeyStorePassword(signatureKeysConfig.KeyStorePassword)
               .WithPrivateKeyPassword(signatureKeysConfig.PrivateKeyPassword)
               .LoadSignatureKeys();
        }
    }
}