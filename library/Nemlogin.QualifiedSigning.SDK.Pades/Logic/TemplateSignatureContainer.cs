using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

using Org.BouncyCastle.Crypto;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic;

/// <summary>
/// IExternalSignatureContainer implementation that computes the CMS signature 
/// to be added to the PDF document using the service provider key/pair.
/// The template signature is later overridden by the signing client.
/// </summary>
public class TemplateSignatureContainer
{
    private const string SIGNING_ALGORITHM = DigestAlgorithms.SHA256;
    // private const string KEY_RESET_SIGNATURE_CONTENT = "nemlogin.signing.pades.reset-signature-content";
    // private const string ENCRYPTION_ALGORITHM = "RSA";

    private readonly PrivateKeySignature _privateKeySignature;
    private readonly ICollection<Org.BouncyCastle.X509.X509Certificate> _certificateChain;

    private TransformationContext _ctx;
    private byte[] _digest;

    public TemplateSignatureContainer(TransformationContext ctx, ICipherParameters pk, ICollection<Org.BouncyCastle.X509.X509Certificate> certificateChain)
    {
        _ctx = ctx;
        _privateKeySignature = new PrivateKeySignature(pk, SIGNING_ALGORITHM); ;
        _certificateChain = certificateChain;
    }

    /// <summary>
    /// Calculate the hash value of the content/data, sets a empty signature on the document
    /// and returns the digest of the encoded PKCS7 signedinfo part.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public byte[] Sign(Stream data)
    {
        string hashAlgorithm = _privateKeySignature.GetHashAlgorithm();

        PdfPkcs7 sgn = new(null, _certificateChain, hashAlgorithm, false);
        byte[] hash = DigestAlgorithms.Digest(data, SIGNING_ALGORITHM);

        byte[] sh = sgn.GetAuthenticatedAttributeBytes(hash, null, null, CryptoStandard.CMS);
        byte[] extSignature = _privateKeySignature.Sign(sh);

        sgn.SetExternalDigest(extSignature, null, _privateKeySignature.GetEncryptionAlgorithm());

        _digest = sgn.GetEncodedPKCS7(hash, CryptoStandard.CMS);

        return _digest;
    }
}
