using Newtonsoft.Json;

namespace BASRemote.Common
{
    /// <inheritdoc />
    public sealed class BasBrowser : IBasBrowser
    {
        /// <inheritdoc />
        [JsonProperty("browser_id", NullValueHandling = NullValueHandling.Ignore)]
        public int? BrowserId { get; set; }

        /// <inheritdoc />
        [JsonProperty("task_id", NullValueHandling = NullValueHandling.Ignore)]
        public int? TaskId { get; set; }
    }
}