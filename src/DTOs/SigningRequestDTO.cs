using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NemLoginSigningWebApp.DTOs
{
    public class SigningRequestDTO
    {
        public SigningDocumentDTO Document { get; set; }

        /// <summary>
        /// Optional: Required subject NameID for the signer.
        /// </summary>
        public string RequiredSigner { get; set; }

        /// <summary>
        /// Output format of signed document
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NemLoginSigningCore.Format.SignatureFormat SignatureFormat { get; set; } = NemLoginSigningCore.Format.SignatureFormat.XAdES;

        public string RequestSignature { get; set; }
    }
}
