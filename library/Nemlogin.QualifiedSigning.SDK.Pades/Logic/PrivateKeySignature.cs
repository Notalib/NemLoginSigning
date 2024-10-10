using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic;

public class PrivateKeySignature
{
    private ICipherParameters pk;
    private string hashAlgorithm;
    private string encryptionAlgorithm;

    public PrivateKeySignature(ICipherParameters pk, string hashAlgorithm)
    {
        this.pk = pk;
        this.hashAlgorithm = DigestAlgorithms.GetDigest(DigestAlgorithms.GetAllowedDigests(hashAlgorithm));
        switch (pk)
        {
            case RsaKeyParameters _:
                this.encryptionAlgorithm = "RSA";
                break;
            case DsaKeyParameters _:
                this.encryptionAlgorithm = "DSA";
                break;
            case ECKeyParameters _:
                this.encryptionAlgorithm = "ECDSA";
                break;
            default:
                throw new ArgumentException($"unknown.key.algorithm.1 {(object) pk.ToString()}");
        }
    }

    public virtual byte[] Sign(byte[] b)
    {
        ISigner signer = SignerUtilities.GetSigner(this.hashAlgorithm + "with" + this.encryptionAlgorithm);
        signer.Init(true, this.pk);
        signer.BlockUpdate(b, 0, b.Length);
        return signer.GenerateSignature();
    }

    public virtual string GetHashAlgorithm() => this.hashAlgorithm;

    public virtual string GetEncryptionAlgorithm() => this.encryptionAlgorithm;
}