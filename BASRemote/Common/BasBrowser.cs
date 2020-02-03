using Newtonsoft.Json;

namespace BASRemote.Common
{
    /// <inheritdoc cref="IBasBrowser" />
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class BasBrowser : IBasBrowser
    {
        /// <inheritdoc />
        [JsonProperty("browser_id")]
        public int? BrowserId { get; set; }

        /// <inheritdoc />
        [JsonProperty("task_id")]
        public int? TaskId { get; set; }
    }
}