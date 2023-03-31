using System;
using System.Diagnostics;
using NemLoginSigningXades.GeneratedSources;
using NemLoginSigningXades.Util;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Format;
using NemLoginSigningCore.Logic;
using NemLoginSigningCore.Utilities;

namespace NemLoginSigningXades.Logic
{
    /// <summary>
    /// Implementation of the ISignatureStamper that updates the DTBS XML SignedDocument 
    /// with a initial Signature element containing SignedInfo with a reference to and 
    /// digest of the DTBS.
    /// </summary>
    public class XmlSignatureStamper : ISignatureStamper
    {
        private readonly string ALGORITHM_CANONICALIZATION = "http://www.w3.org/2001/10/xml-exc-c14n#";
        private readonly string ALGORITHM_SIGNATURE = "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha256";
        private readonly string REFERENCE_TYPE_SIGN_TEXT = "http://dk.gov.certifikat/nemlogin#SignText";
        private readonly string ALGORITHM_DIGEST = "http://www.w3.org/2001/04/xmlenc#sha256";
        
        public bool CanSign(SignatureFormat signatureFormat)
        {
            return signatureFormat == SignatureFormat.XAdES;
        }

        public void PresignDocument(TransformationContext ctx)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException(nameof(ctx));
            }

            // Add initial XML Signature
            SignedDocumentType signedDocumentType = XMLSerializer.Deserialize<SignedDocumentType>(ctx.DataToBeSigned.GetData());

            signedDocumentType.Signature = CreateSignature(signedDocumentType);

            XMLSerializer.Serialize(signedDocumentType);

            // Update Dtbs XML Document
            ctx.DataToBeSigned = new XadesDataToBeSigned(XMLSerializer.Serialize(signedDocumentType), ctx.DataToBeSigned.FileName);

            // Update the signature parameters with the SignedInfo element
            string signatureEncoded = Convert.ToBase64String(XMLSerializer.Serialize(signedDocumentType.Signature));

            Debug.WriteLine($"SignedInfo Signature Encoded: {signatureEncoded}");
            ctx.UpdateDtbsSignedInfo(signatureEncoded);
        }

        private SignatureType CreateSignature(SignedDocumentType signedDocumentType)
        {
            var serializedSignText = XMLSerializer.Serialize(signedDocumentType.SignText);

            Byte[] digestValueBeforeCanonicalize = CryptographyLogic.ComputeSha256Hash(serializedSignText);

            var beforeCanonicalise = Convert.ToBase64String(digestValueBeforeCanonicalize);
            Debug.WriteLine($"Digest Before Canonicalized: {beforeCanonicalise}");

            // Canonicalize
            var canonicalizedResult = CryptographyLogic.Canonicalize(serializedSignText);
            
            // Digest Sha256
            Byte[] digestValue = CryptographyLogic.ComputeSha256Hash(canonicalizedResult);

            var afterCanonicalise = Convert.ToBase64String(digestValue);
            Debug.WriteLine($"Digest After Canonicalized: {afterCanonicalise}");

            signedDocumentType.SignText = XMLSerializer.Deserialize<SignTextType>(canonicalizedResult);

            string signatureId =$"id-{Guid.NewGuid().ToString()}";

            SignatureType signatureType = new SignatureType();
            signatureType.WithId(signatureId)
                .WithSignedInfo(new SignedInfoType()
                .withCanonicalizationMethod(new CanonicalizationMethodType { Algorithm = ALGORITHM_CANONICALIZATION })
                .WithSignatureMethod(new SignatureMethodType { Algorithm = ALGORITHM_SIGNATURE })
                .WithReference(CreateSignTextReference(signatureId, signedDocumentType.SignText.id, digestValue)));

            return signatureType;
        }

        private ReferenceType CreateSignTextReference(string signatureId, string signTextId, byte[] digestValue)
        {
            string id = $"r-{signatureId}-1";
            string uri = $"#{signTextId}";
            string type = REFERENCE_TYPE_SIGN_TEXT;

            DigestMethodType digestMethodType = new DigestMethodType { Algorithm = ALGORITHM_DIGEST };

            ReferenceType referenceType = new ReferenceType { Id = id, URI = uri, Type = type, DigestMethod = digestMethodType, DigestValue = digestValue };

            // Add Canonicalization algorithm
            referenceType.WithTransforms(TransformsType.CreateTransformsType()
                .WithTransform(TransformType.CreateTransformType().
                        WithAlgorithm(ALGORITHM_CANONICALIZATION)));

            return referenceType;
        }
    }
}