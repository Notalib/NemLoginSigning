using System.Net.Http;
using System.Net.Http.Json;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using NemLoginSigningWebApp.Config;
using NemLoginSigningDTO.UUIDMatch;

namespace NemLoginSigningWebApp.Logic;

public class UUIDMatchClient : IUUIDMatchClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    private readonly HttpClient _httpClient;
    private readonly NemloginConfig _nemLogInConfiguration;

    static UUIDMatchClient()
    {
        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public UUIDMatchClient(HttpClient httpClient, IOptions<NemloginConfig> nemLogInConfiguration)
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
