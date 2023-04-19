namespace NemLoginSigningPades.Logic
{
    /// <summary>
    /// Data transfer class to use internally in PdfSignatureStamper for returning multiple values
    /// </summary>
    public class PresignResult
    {
        public PresignResult(byte[] signedResult, byte[] signature)
        {
            SignedResult = signedResult;
            Signature = signature;
        }

        public byte[] SignedResult { get; private set; }

        public byte[] Signature { get; set; }
    }
}
