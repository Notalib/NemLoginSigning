namespace NemLoginSigningXades.GeneratedSources
{
    public partial class SignatureType
    {
        public SignatureType WithId(string signatureId)
        {
            Id = signatureId;
            return this;
        }

        public SignatureType WithSignedInfo(SignedInfoType signedInfoType)
        {
            SignedInfo = signedInfoType;
            return this;
        }
    }
}
