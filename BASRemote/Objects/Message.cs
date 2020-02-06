using BASRemote.Helpers;
using Newtonsoft.Json;

namespace BASRemote.Objects
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Message
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="async"></param>
        internal Message(dynamic data, string type, bool async)
        {
            Id = Rand.NextInt(100000, 999999);
            Async = async;
            Type = type;
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        internal Message()
        {
        }

        /// <summary>
        ///     Message data object.
        /// </summary>
        [JsonProperty("data")]
        public dynamic Data { get; set; }

        /// <summary>
        ///     Message type string.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        ///     Is message async.
        /// </summary>
        [JsonProperty("async")]
        public bool Async { get; set; }

        /// <summary>
        ///     Message id number.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}