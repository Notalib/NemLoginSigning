using System.IO;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.DTO;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NemLoginSigningWebApp.Models
{
    /// <summary>
    /// Used for actually storing the signingpayloadDTO and showing the document-to-be-signed.
    /// Used as model for the Sign.cshtml view.
    /// </summary>
    public class SigningModel : ViewModelBase
    {
        public SigningModel()
        {
        }

        public SigningModel(SigningPayloadDTO signingPayloadDTO)
        {
            SigningPayloadDTO = signingPayloadDTO;
        }

        public SigningModel(SigningPayloadDTO signingPayloadDTO, string signingClientURL, SignersDocument signersDocument, string format)
        {
            SigningPayloadDTO = signingPayloadDTO;
            SigningClientURL = signingClientURL;
            SignersDocument = signersDocument;
            Format = format;
        }

        public SigningPayloadDTO SigningPayloadDTO { get; private set; }

        public string SigningClientURL { get; set; }

        public SignersDocument SignersDocument { get; set; }

        public string Format { get; set; }

        public HtmlString SerializedHTMLString()
        {
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                var serializer = new JsonSerializer
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                serializer.Serialize(jsonWriter, SigningPayloadDTO);

                var htmlString = new HtmlString(stringWriter.ToString());

                return htmlString;
            }
        }
    }
}