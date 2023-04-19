using NemLoginSigningCore.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NemLoginSigningWebApp.DTOs
{
    public class SigningRequestDTO
    {
        /// <summary>
        /// Document to be signed.
        /// </summary>
        public SigningDocumentDTO Document { get; set; }

        /// <summary>
        /// Language to present the signing client in.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Language Language { get; set; }

        /// <summary>
        /// Optional: Required subject NameID for the signer.
        /// </summary>
        public string RequiredSigner { get; set; }

        /// <summary>
        /// Output format of signed document
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NemLoginSigningCore.Format.SignatureFormat SignatureFormat { get; set; } = NemLoginSigningCore.Format.SignatureFormat.XAdES;

        /// <summary>
        /// Signature of the signing document from a trusted backend.
        /// </summary>
        public string RequestSignature { get; set; }
    }
}
