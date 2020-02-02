using Newtonsoft.Json;

namespace BASRemote.Common
{
    /// <inheritdoc cref="IBasTask" />
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class BasTask : IBasTask
    {
        /// <inheritdoc />
        [JsonProperty("started_at")]
        public int StartedAt { get; set; }

        /// <inheritdoc />
        [JsonProperty("browser_id")]
        public int? BrowserId { get; set; }

        /// <inheritdoc />
        [JsonProperty("task_id")]
        public int? TaskId { get; set; }
    }
}