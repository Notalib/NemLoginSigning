using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nemlogin.QualifiedSigning.Common.Helpers;

public static class SerializationExtensions
{
    public static string ToJson<TEntity>(this TEntity entity)
    {
        using (StringWriter stringWriter = new StringWriter())
        using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
        {
            JsonSerializer serializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            serializer.Serialize(jsonWriter, entity);

            return stringWriter.ToString();
        }
    }
}