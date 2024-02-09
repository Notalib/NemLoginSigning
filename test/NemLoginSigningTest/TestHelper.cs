using System.IO;

using Microsoft.Extensions.Configuration;
using NemLoginSigningCore.Configuration;

namespace NemloginSigningTest
{
    /// <summary>
    /// Helper class for general configuration and setup of test properties
    /// </summary>
    public static class TestHelper
    {
        public static NemloginConfiguration GetConfiguration(string section)
        {
            IConfigurationRoot configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            IConfigurationSection config = configBuilder.GetSection(section);

            NemloginConfiguration configuration = new NemloginConfiguration();
            ConfigurationBinder.Bind(config, configuration);

            return configuration;
        }
    }
}
