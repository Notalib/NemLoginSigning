using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Configuration;
using NemLoginSigningCore.Logging;
using NemLoginSigningWebApp;
using Xunit;

namespace NemloginSigningTest
{
    public class WebAppTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly Uri _apiUrl;

        [Fact]
        public async Task TestNemloginSigningWebAppConfiguration()
        {
            Uri uri = new Uri(_apiUrl + "IsAlive");

            HttpResponseMessage response = await _client.GetAsync(uri);

            string result = await response.Content.ReadAsStringAsync();

            Assert.Equal("NemloginSigningWebApp is up and running.", result);
        }

        public WebAppTests()
        {
            _server = new TestServer(new WebHostBuilder()
              .ConfigureAppConfiguration(config => config.AddJsonFile("appsettings.json"))
              .UseEnvironment(Environments.Development)
              .ConfigureServices(services => services.Configure<NemloginConfiguration>(GetAppSettingsSection(TestConstants.ConfigurationNemlogin)))
              .UseStartup<Startup>());

            LoggerCreator.LoggerFactory = _server.Services.GetRequiredService<ILoggerFactory>();

            ILogger logger = LoggerCreator.CreateLogger<WebAppTests>();

            _client = _server.CreateClient();

            _apiUrl = new Uri("https://localhost:44358/Home/");

            logger.LogInformation($"SignSdk - TestServer Setup Done - Server running");
        }

        public IConfigurationSection GetAppSettingsSection(string section)
        {
            IConfigurationRoot result = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json").Build();

            return result.GetSection(section);
        }
    }
}
