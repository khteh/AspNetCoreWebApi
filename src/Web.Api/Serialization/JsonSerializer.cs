using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Web.Api.Serialization
{
    public sealed class JsonSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,//System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full,
            ContractResolver = new JsonContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string SerializeObject(object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented, Settings);
        }
        public static T DeSerializeObject<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str, Settings);
        }

        public sealed class JsonContractResolver : CamelCasePropertyNamesContractResolver
        {
        }
    }
}