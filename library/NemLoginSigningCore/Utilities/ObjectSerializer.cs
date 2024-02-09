using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NemLoginSigningCore.Utilities
{
    /// <summary>
    /// Utility class for generic serializing and deserializing.
    /// </summary>
    public static class ObjectSerializer
    {
        public static string SerializeObject(object value)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return JsonConvert.SerializeObject(value, Formatting.None, jsonSerializerSettings);
        }

        public static T DeSerializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
