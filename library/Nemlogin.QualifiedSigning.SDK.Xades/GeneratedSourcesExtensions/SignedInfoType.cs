namespace Nemlogin.QualifiedSigning.SDK.Xades.GeneratedSources;

public partial class SignedInfoType
{
    public SignedInfoType WithCanonicalizationMethod(CanonicalizationMethodType canonicalizationMethodType)
    {
        CanonicalizationMethod = canonicalizationMethodType;
        return this;
    }

    public SignedInfoType WithSignatureMethod(SignatureMethodType signatureMethod)
    {
        SignatureMethod = signatureMethod;
        return this;
    }

    public SignedInfoType WithReference(ReferenceType referenceType)
    {
        Reference = new[] { referenceType };
        return this;
    }
}