using System.Net.Http;
using System.Net.Http.Json;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using NemLoginSigningDTO.UUIDMatch;
using NemLoginSigningCore.Exceptions;
using NemLoginSigningCore.Configuration;
using Microsoft.Extensions.Options;

namespace NemLoginSigningWebApp.Logic
{
    public class UUIDMatchClient : IUUIDMatchClient
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };

        private readonly HttpClient _httpClient;
        private readonly NemloginConfiguration _nemLogInConfiguration;

        static UUIDMatchClient()
        {
            JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public UUIDMatchClient(HttpClient httpClient, IOptions<NemloginConfiguration> nemLogInConfiguration)
        {
            _httpClient = httpClient;
            _nemLogInConfiguration = nemLogInConfiguration.Value;
        }

        public async Task<SubjectMatchesSignerResult> SubjectMatchesSigner(SubjectMatchesSignerDTO request)
        {
            request.ServiceProviderEntityID = _nemLogInConfiguration.EntityID;

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/uuidmatch/subjectMatchesSigner", request, JsonSerializerOptions);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                throw new NemLoginException($"SubjectMatchesSigner call failed: {errorMessage}");
            }

            return await response.Content.ReadFromJsonAsync<SubjectMatchesSignerResult>(JsonSerializerOptions);
        }
    }
}
