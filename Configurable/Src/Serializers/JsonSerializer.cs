using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Configurable.Serializers
{
    public class JsonSerializer : ISerializer
    {
        public static readonly JsonSerializer Instance = new JsonSerializer();

        public T Deserialize<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new StringEnumConverter());
        }
    }
}
