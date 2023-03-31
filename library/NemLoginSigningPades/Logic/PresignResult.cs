namespace NemLoginSigningPades.Logic
{
    /// <summary>
    /// Data transfer class to use internally in PdfSignatureStamper for returning multiple values
    /// </summary>
    public class PresignResult
    {
        public PresignResult(byte[] SignedResult, byte[] Signature)
        {
            this.SignedResult = SignedResult;   
            this.Signature = Signature;
        }

        public byte[] SignedResult { get; private set; }

        public byte[] Signature { get; set; }
    }
}
