using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic;

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

        // Generate key pair
        AsymmetricCipherKeyPair keyPair = GenerateKeyPair();

        // Generate certificate
        X509Certificate2 certificate = GenerateCertificate(keyPair);

#pragma warning disable CA1416
        RSA rsaPrivate = RSA.Create(2048);
#pragma warning restore CA1416

        return new SignatureKeys(certificate, rsaPrivate);
    }

    static AsymmetricCipherKeyPair GenerateKeyPair()
    {
        RsaKeyPairGenerator generator = new RsaKeyPairGenerator();
        KeyGenerationParameters keyGenerationParameters = new KeyGenerationParameters(new SecureRandom(), 2048);
        generator.Init(keyGenerationParameters);
        return generator.GenerateKeyPair();
    }

    static X509Certificate2 GenerateCertificate(AsymmetricCipherKeyPair keyPair)
    {
        // Generate certificate
        X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();
        BigInteger serialNumber = BigInteger.ProbablePrime(120, new Random());

        X509Name x509Name = new X509Name(DN);
        certificateGenerator.SetIssuerDN(x509Name);
        certificateGenerator.SetSerialNumber(new Org.BouncyCastle.Math.BigInteger(1 + DateTime.Now.Millisecond.ToString()));
        certificateGenerator.SetNotBefore(DateTime.Now.AddDays(-1));
        certificateGenerator.SetNotAfter(DateTime.Now.AddDays(1));
        certificateGenerator.SetSubjectDN(x509Name);
        certificateGenerator.SetPublicKey(keyPair.Public);

        certificateGenerator.AddExtension(new DerObjectIdentifier("2.5.29.19"), false, new BasicConstraints(false));
        certificateGenerator.AddExtension(new DerObjectIdentifier("2.5.29.15"), true, new X509KeyUsage(
            X509KeyUsage.DigitalSignature | X509KeyUsage.NonRepudiation |
            X509KeyUsage.KeyEncipherment | X509KeyUsage.DataEncipherment));

        Asn1SignatureFactory signatureFactory = new Asn1SignatureFactory("SHA256WITHRSA", keyPair.Private);
        Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(signatureFactory);

        // Convert to X509Certificate2
        byte[] certData = certificate.GetEncoded();
        X509Certificate2 x509Certificate = new X509Certificate2(certData);
        return x509Certificate;
    }
}