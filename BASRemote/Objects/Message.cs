using BASRemote.Helpers;
using Newtonsoft.Json;

namespace BASRemote.Objects
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class Message
    {
        public Message(dynamic data, string type, bool async)
        {
            Id = Rand.NextInt(100000, 999999);
            Async = async;
            Type = type;
            Data = data;
        }

        public Message()
        {
        }

        [JsonProperty("data")] 
        public dynamic Data { get; set; }

        [JsonProperty("type")] 
        public string Type { get; set; }

        [JsonProperty("async")] 
        public bool Async { get; set; }

        [JsonProperty("id")] 
        public int Id { get; set; }
    }
}