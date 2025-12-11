using System.Text.Json;
using System.Text.Json.Serialization;
namespace Web.Api.Serialization;

public sealed class JsonSerializer
{
    private static JsonSerializerOptions Options = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    public static string SerializeObject(object o) => System.Text.Json.JsonSerializer.Serialize(o, Options);
    public static T? DeSerializeObject<T>(string str) => System.Text.Json.JsonSerializer.Deserialize<T>(str, Options);
}