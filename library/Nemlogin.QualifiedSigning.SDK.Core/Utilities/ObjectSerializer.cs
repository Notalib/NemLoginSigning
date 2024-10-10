using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nemlogin.QualifiedSigning.SDK.Core.Utilities;

/// <summary>
/// Utility class for generic serializing and deserializing.
/// </summary>
public class ObjectSerializer
{
    public static string SerializeObject(object value)
    {
        JsonSerializerSettings jsonSerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        return JsonConvert.SerializeObject(value, Formatting.None, jsonSerializerSettings);
    }

    public static T DeSerializeObject<T>(string value)
    {
        return JsonConvert.DeserializeObject<T>(value);
    }
}