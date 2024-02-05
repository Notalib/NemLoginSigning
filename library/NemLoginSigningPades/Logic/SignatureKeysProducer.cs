using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using NemLoginSigningCore.Core;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace NemLoginSigningPades.Logic
{
    /// <summary>
    ///  Utility class used for generating the private key and certificate used for
    ///  pre-signing the DTBS(Data TO Be Signed) document.
    ///  The private key + certificate will be returned in the form of a SignatureKeys object.
    ///  The CMS signature will subsequently be replaced by the Signing Client in the user browser.
    /// </summary>
    public class SignatureKeysProducer
    {
        private const string DN = "cn=NemLog-In";

        /// <summary>
        /// Generates a new private key and certificate
        /// </summary>
        /// <returns>SignatureKeys wrapping the Certificate represented as an X509Certificate2 object</returns>
        public SignatureKeys CreateSignatureKeys()
        {
            RsaKeyPairGenerator keypairgen = new RsaKeyPairGenerator();

            keypairgen.Init(new KeyGenerationParameters(new SecureRandom(), 1024));

            AsymmetricCipherKeyPair keypair = keypairgen.GenerateKeyPair();

            var x509Name = new Org.BouncyCastle.Asn1.X509.X509Name(DN);

            X509V3CertificateGenerator generator = new X509V3CertificateGenerator();
            generator.SetIssuerDN(x509Name);
            generator.SetSerialNumber(new Org.BouncyCastle.Math.BigInteger(1 + DateTime.Now.Millisecond.ToString()));
            generator.SetNotBefore(DateTime.Now.AddDays(-1));
            generator.SetNotAfter(DateTime.Now.AddDays(1));
            generator.SetSubjectDN(x509Name);
            generator.SetSignatureAlgorithm("sha512WithRSA");
            generator.SetPublicKey(keypair.Public);

            generator.AddExtension(new DerObjectIdentifier("2.5.29.19"), false, new BasicConstraints(false));
            generator.AddExtension(new DerObjectIdentifier("2.5.29.15"), true, new X509KeyUsage(X509KeyUsage.DigitalSignature | X509KeyUsage.NonRepudiation |
                                                                                             X509KeyUsage.KeyEncipherment | X509KeyUsage.DataEncipherment));

            var x509Certificate = GenerateX509Certificate(generator, keypair);

            RSA rsaPriv = DotNetUtilities.ToRSA(keypair.Private as RsaPrivateCrtKeyParameters);

            return new SignatureKeys(x509Certificate, rsaPriv);
        }

        private X509Certificate2 GenerateX509Certificate(X509V3CertificateGenerator generator, AsymmetricCipherKeyPair keypair)
        {
            Org.BouncyCastle.X509.X509Certificate certificate = generator.Generate(keypair.Private);

            X509Certificate2 tempX509Certificate2 = new X509Certificate2(certificate.GetEncoded());

            return tempX509Certificate2;
        }
    }
}
