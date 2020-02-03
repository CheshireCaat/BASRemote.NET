using System;
using Newtonsoft.Json;

namespace BASRemote.Objects
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class Message
    {
        private static readonly Random Rand = new Random();

        public Message(dynamic data, string type, bool async)
        {
            Id = Rand.Next(100000, 999999);
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

        public static Message FromJson(string message)
        {
            return JsonConvert.DeserializeObject<Message>(message);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}