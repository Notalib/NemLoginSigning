using NemLoginSigningCore.Format;

namespace NemLoginSigningCore.Core
{
    /// <summary>
    /// Specification of SignersDocument for Text documents.
    /// </summary>
    public class PlainTextSignersDocument : SignersDocument
    {
        public PlainTextSignersDocument(SignersDocumentFile signersDocumentFile, SignProperties signProperties)
            : base(DocumentFormat.TEXT, signersDocumentFile, signProperties)
        {
        }

        public PlainTextSignersDocument(SignersDocumentFile signersDocumentFile, bool useMonoSpaceFont)
            : base(DocumentFormat.TEXT, signersDocumentFile, null)
        {
            UseMonoSpaceFont = useMonoSpaceFont;
        }

        public PlainTextSignersDocument(SignersDocumentFile signersDocumentFile, bool useMonoSpaceFont, SignProperties signProperties)
            : base(DocumentFormat.TEXT, signersDocumentFile, signProperties)
        {
            UseMonoSpaceFont = useMonoSpaceFont;
        }

        public bool UseMonoSpaceFont { get; set; }
    }
}
