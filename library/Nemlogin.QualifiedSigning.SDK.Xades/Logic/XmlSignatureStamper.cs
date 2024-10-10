﻿using System.Diagnostics;

using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Logic;
using Nemlogin.QualifiedSigning.SDK.Core.Utilities;
using Nemlogin.QualifiedSigning.SDK.Xades.GeneratedSources;
using Nemlogin.QualifiedSigning.SDK.Xades.Utilities;

namespace Nemlogin.QualifiedSigning.SDK.Xades.Logic;

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

    public void PreSignDocument(TransformationContext ctx)
    {
        if (ctx == null)
        {
            throw new ArgumentNullException(nameof(ctx));
        }

        // Add initial XML Signature
        SignedDocumentType signedDocumentType = XmlSerializer.Deserialize<SignedDocumentType>(ctx.DataToBeSigned.GetData());

        signedDocumentType.Signature = CreateSignature(signedDocumentType);

        XmlSerializer.Serialize(signedDocumentType);

        // Update Dtbs XML Document
        ctx.DataToBeSigned = new XadesDataToBeSigned(XmlSerializer.Serialize(signedDocumentType), ctx.DataToBeSigned.FileName);

        // Update the signature parameters with the SignedInfo element
        string signatureEncoded = Convert.ToBase64String(XmlSerializer.Serialize(signedDocumentType.Signature));

        Debug.WriteLine($"SignedInfo Signature Encoded: {signatureEncoded}");
        ctx.UpdateDtbsSignedInfo(signatureEncoded);
    }

    private SignatureType CreateSignature(SignedDocumentType signedDocumentType)
    {
        byte[] serializedSignText = XmlSerializer.Serialize(signedDocumentType.SignText);

        Byte[] digestValueBeforeCanonicalize = CryptographyLogic.ComputeSha256Hash(serializedSignText);

        string beforeCanonicalize = Convert.ToBase64String(digestValueBeforeCanonicalize);
        Debug.WriteLine($"Digest Before Canonicalized: {beforeCanonicalize}");

        // Canonicalize
        byte[] canonicalizedResult = CryptographyLogic.Canonicalize(serializedSignText);

        // Digest Sha256
        Byte[] digestValue = CryptographyLogic.ComputeSha256Hash(canonicalizedResult);

        string afterCanonicalize = Convert.ToBase64String(digestValue);
        Debug.WriteLine($"Digest After Canonicalized: {afterCanonicalize}");

        signedDocumentType.SignText = XmlSerializer.Deserialize<SignTextType>(canonicalizedResult);

        string signatureId = $"id-{Guid.NewGuid().ToString()}";

        SignatureType signatureType = new SignatureType();
        signatureType.WithId(signatureId)
            .WithSignedInfo(new SignedInfoType()
            .WithCanonicalizationMethod(new CanonicalizationMethodType { Algorithm = ALGORITHM_CANONICALIZATION })
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