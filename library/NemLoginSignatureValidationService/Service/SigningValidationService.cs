using System;
using System.Net.Http;
using System.Threading.Tasks;

using NemLoginSignatureValidationService.Model;
using NemLoginSigningCore.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NemLoginSignatureValidationService.Service
{
    /// <summary>
    /// Simple implementation of a service for validating a signed document
    /// by calling the public NemLog-In Signature Validation API.
    /// </summary>
    public class SigningValidationService : ISigningValidationService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SigningValidationService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// Calls the NemLog-In Signature Validation API and return a ValidationReport as a result.
        /// </summary>
        /// <returns>ValidationReport</returns>
        public async Task<ValidationReport> Validate(SignatureValidationContext ctx)
        {
            ValidationReport validationReport = null;

            try
            {
                using (HttpClient httpClient = _httpClientFactory.CreateClient("ValidationServiceClient"))
                using (ByteArrayContent byteArrayContent = new ByteArrayContent(ctx.GetDocumentData()))
                using (MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent())
                {
                    if (ctx.Timeout > 0)
                    {
                        httpClient.Timeout = new TimeSpan(0, 0, ctx.Timeout);
                    }

                    httpClient.BaseAddress = new Uri(ctx.ValidationServiceUrl);

                    multipartFormDataContent.Add(byteArrayContent, "file", ctx.DocumentName);

                    HttpResponseMessage result = await httpClient.PostAsync("api/validate", multipartFormDataContent);

                    if (result.IsSuccessStatusCode)
                    {
                        HttpContent content = result.Content;

                        string json = await result.Content.ReadAsStringAsync();

                        validationReport = JsonConvert.DeserializeObject<ValidationReport>(json, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            TypeNameHandling = TypeNameHandling.Auto,
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
                    }
                }

                return validationReport;
            }
            catch (Exception e)
            {
                throw new NemLoginException("Error validating signature", ErrorCode.SDK011, e);
            }
        }
    }
}