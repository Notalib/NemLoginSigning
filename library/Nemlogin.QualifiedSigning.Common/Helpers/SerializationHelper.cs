using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nemlogin.QualifiedSigning.Common.Helpers;

public static class SerializationExtensions
{
    public static string ToJson<TEntity>(this TEntity entity)
    {
        using (var stringWriter = new StringWriter())
        using (var jsonWriter = new JsonTextWriter(stringWriter))
        {
            var serializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            serializer.Serialize(jsonWriter, entity);
                                
            return stringWriter.ToString();
        }
    }
}