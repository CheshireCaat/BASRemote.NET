using BASRemote.Helpers;
using Newtonsoft.Json;

namespace BASRemote.Objects
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class Message
    {
        [JsonProperty("data")] 
        public dynamic Data { get; set; }

        [JsonProperty("type")] 
        public string Type { get; set; }

        [JsonProperty("async")] 
        public bool Async { get; set; }

        [JsonProperty("id")] 
        public int Id { get; set; }

        public Message(dynamic data, string type, bool async) : this()
        {
            Async = async;
            Type = type;
            Data = data;
        }

        public Message()
        {
            Id = RandomHelper.GenerateId();
        }
    }
}