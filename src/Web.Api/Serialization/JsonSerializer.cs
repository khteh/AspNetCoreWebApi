using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Web.Api.Serialization
{
    public sealed class JsonSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            ContractResolver = new JsonContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
        };
        public static string SerializeObject(object o) => JsonConvert.SerializeObject(o, Formatting.Indented, Settings);
        public static T DeSerializeObject<T>(string str) => JsonConvert.DeserializeObject<T>(str, Settings);
        public sealed class JsonContractResolver : CamelCasePropertyNamesContractResolver
        {
        }
    }
}