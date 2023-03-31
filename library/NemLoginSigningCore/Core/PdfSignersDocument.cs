namespace NemLoginSigningCore.Core
{
    /// <summary>
    /// Specification of SignersDocument for PDF documents.
    /// </summary>
    public class PdfSignersDocument : SignersDocument
    {
        public PdfSignersDocument(SignersDocumentFile signersDocumentFile)
            : this(signersDocumentFile, null)
        {
        }

        public PdfSignersDocument(SignersDocumentFile signersDocumentFile, SignProperties signProperties)
            : base(Format.DocumentFormat.PDF, signersDocumentFile, signProperties)
        {
        }
    }
}
