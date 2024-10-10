using System;
using System.Linq;
using System.Text;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Utilities;

namespace Nemlogin.QualifiedSigning.SDK.Tests
{
    public abstract class SigningTestBase
    {
        public SigningTestBase()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public string EntityID
        {
            get
            {
                if (!TestHelper.GetConfiguration().SignatureKeysConfiguration.Any())
                {
                    throw new InvalidOperationException("No signaturekeys configuration");
                }
                        
                var entityID = TestHelper.GetConfiguration().SignatureKeysConfiguration.First().EntityId;

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
            var configuration = TestHelper.GetConfiguration();

            if (!configuration.SignatureKeysConfiguration.Any())
            {
                throw new InvalidOperationException("No signaturekeys configuration");
            }

            var signatureKeysConfig = configuration.SignatureKeysConfiguration.First();

            return new SignatureKeysLoader()
               .WithKeyStorePath(signatureKeysConfig.KeystorePath)
               .WithKeyStorePassword(signatureKeysConfig.KeystorePassword)
               .WithPrivateKeyPassword(signatureKeysConfig.PrivateKeyPassword)
               .LoadSignatureKeys();
        }
    }
}