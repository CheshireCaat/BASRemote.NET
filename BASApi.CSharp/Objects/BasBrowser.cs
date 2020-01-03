using Newtonsoft.Json;

namespace BASApi.CSharp.Objects
{
    /// <inheritdoc />
    public sealed class BasBrowser : IBasBrowser
    {
        /// <inheritdoc />
        [JsonProperty("browser_id")]
        public int BrowserId { get; set; }

        /// <inheritdoc />
        [JsonProperty("task_id")]
        public int TaskId { get; set; }
    }
}