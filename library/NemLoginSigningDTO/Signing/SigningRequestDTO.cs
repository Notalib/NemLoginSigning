namespace NemLoginSigningDTO.Signing
{
    public class SigningRequestDTO
    {
        /// <summary>
        /// User specified signing request identifier.
        /// </summary>
        public string RequestID { get; set; }

        /// <summary>
        /// Document to be signed.
        /// </summary>
        public SigningDocumentDTO Document { get; set; }

        /// <summary>
        /// Header shown in IFrame signing client.
        /// </summary>
        public string ReferenceText { get; set; }

        /// <summary>
        /// Language to present the signing client in.
        /// </summary>
        public string Language { get; set; } = "da";

        /// <summary>
        /// Optional: Required subject NameID for the signer.
        /// </summary>
        public string RequiredSigner { get; set; }

        /// <summary>
        /// Output format of signed document
        /// </summary>
        public string SignatureFormat { get; set; } = "XAdES";

    }
}
