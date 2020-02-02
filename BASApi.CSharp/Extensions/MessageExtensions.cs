using BASRemote.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BASRemote.Extensions
{
    public static class MessageExtensions
    {
        public static Message FromJson(this string message)
        {
            return JsonConvert.DeserializeObject<Message>(message);
        }

        public static JObject ToJObject(this Message message)
        {
            return JObject.FromObject(message);
        }

        public static string ToJson(this Message message)
        {
            return JsonConvert.SerializeObject(message);
        }
    }
}