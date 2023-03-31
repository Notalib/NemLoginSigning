using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using NemLoginSigningCore.Configuration;
using NemLoginSigningCore.Utilities;

namespace NemloginSigningTest
{
    /// <summary>
    /// Helper class for general configuration and setup of test properties
    /// </summary>
    public static class TestHelper
    {
        public static NemloginConfiguration GetConfiguration(string section)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var config = configBuilder.GetSection(section);

            NemloginConfiguration configuration = new NemloginConfiguration();
            ConfigurationBinder.Bind(config, configuration);

            return configuration;
        }
    }
}