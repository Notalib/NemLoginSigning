using System.Collections.Generic;
using System.IO;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using NemLoginSigningCore.Core;
using Org.BouncyCastle.Crypto;

namespace NemLoginSigningPades.Logic
{
    /// <summary>
    /// IExternalSignatureContainer implementation that computes the CMS signature
    /// to be added to the PDF document using the service provider key/pair.
    /// The template signature is later overridden by the signing client.
    /// </summary>
    public class TemplateSignatureContainer : IExternalSignatureContainer
    {
        private const string SIGNING_ALGORITHM = DigestAlgorithms.SHA256;
        private const string ENCRYPTION_ALGORITHM = "RSA";

        private readonly PrivateKeySignature _privateKeySignature;
        private readonly ICollection<Org.BouncyCastle.X509.X509Certificate> _certificateChain;
        private byte[] _digest;

        public byte[] Digest => _digest;

        public TemplateSignatureContainer(TransformationContext ctx, ICipherParameters pk, ICollection<Org.BouncyCastle.X509.X509Certificate> certificateChain)
        {
            _privateKeySignature = new PrivateKeySignature(pk, SIGNING_ALGORITHM);
            _certificateChain = certificateChain;
        }

        public string GetHashAlgorithm()
        {
            return SIGNING_ALGORITHM;
        }

        public string GetEncryptionAlgorithm()
        {
            return ENCRYPTION_ALGORITHM;
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

            PdfPKCS7 sgn = new PdfPKCS7(null, _certificateChain, hashAlgorithm, false);
            byte[] hash = DigestAlgorithms.Digest(data, SIGNING_ALGORITHM);

            byte[] sh = sgn.getAuthenticatedAttributeBytes(hash, null, null, CryptoStandard.CMS);
            byte[] extSignature = _privateKeySignature.Sign(sh);

            sgn.SetExternalDigest(extSignature, null, _privateKeySignature.GetEncryptionAlgorithm());

            _digest = sgn.GetEncodedPKCS7(hash, null, null, null, CryptoStandard.CMS);

            return _digest;
        }

        /// <summary>
        /// Add filter and default subfilter to the signature dictionary for Pades Signing
        /// </summary>
        /// <param name="signDic"></param>
        public void ModifySigningDictionary(PdfDictionary signDic)
        {
            signDic.Put(PdfName.FILTER, PdfSignatureStamper.SIGNATURE_DEFAULT_FILTER);
            signDic.Put(PdfName.SUBFILTER, PdfSignatureStamper.SIGNATURE_DEFAULT_SUBFILTER);
        }
    }
}
