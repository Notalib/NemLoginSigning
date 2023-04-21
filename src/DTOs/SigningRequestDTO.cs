using System.Text.Json.Serialization;

using NemLoginSigningCore.Core;

namespace NemLoginSigningWebApp.DTOs
{
    public class SigningRequestDTO
    {
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
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Language Language { get; set; }

        /// <summary>
        /// Optional: Required subject NameID for the signer.
        /// </summary>
        public string RequiredSigner { get; set; }

        /// <summary>
        /// Output format of signed document
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NemLoginSigningCore.Format.SignatureFormat SignatureFormat { get; set; } = NemLoginSigningCore.Format.SignatureFormat.XAdES;

        /// <summary>
        /// Signature of the signing document from a trusted backend.
        /// </summary>
        public string RequestSignature { get; set; }
    }
}
