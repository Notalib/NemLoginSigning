using System.IO;
using Nemlogin.QualifiedSigning.SDK.Core.Configuration;
using Newtonsoft.Json;

namespace Nemlogin.QualifiedSigning.SDK.Tests
{
    /// <summary>
    /// Helper class for general configuration and setup of test properties
    /// </summary>
    public class TestHelper
    {
        public static NemloginConfiguration GetConfiguration()
        {
            // This is done to support loading the appsettings.json from both .NET Core and .NET Framework
            var appsettingsContent = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
            var config = JsonConvert.DeserializeObject<NemloginConfiguration>(appsettingsContent);
            return config;
        }
    }
}