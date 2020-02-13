using BASRemote.Helpers;
using Newtonsoft.Json;

namespace BASRemote.Objects
{
    /// <summary>
    ///     Class that represents default BAS message.
    /// </summary>
    public sealed class Message
    {
        /// <summary>
        ///     Create an instance of <see cref="Message" /> class.
        /// </summary>
        /// <param name="data">
        ///     Message data object.
        /// </param>
        /// <param name="type">
        ///     Message type string.
        /// </param>
        /// <param name="async">
        ///     Is message async.
        /// </param>
        internal Message(dynamic data, string type, bool async)
        {
            Id = Rand.NextInt(100000, 999999);
            Async = async;
            Type = type;
            Data = data;
        }

        /// <summary>
        ///     Create an instance of <see cref="Message" /> class.
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