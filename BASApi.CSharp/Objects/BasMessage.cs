using Newtonsoft.Json;

namespace BASApi.CSharp.Objects
{
    internal class BasMessage
    {
        [JsonProperty("data")] public BasMessageData Data { get; set; }

        [JsonProperty("type")] public string Type { get; set; }
    }

    internal class BasMessageData
    {
        [JsonProperty("browser_id")] public int? BrowserId { get; set; }

        [JsonProperty("task_id")] public int? TaskId { get; set; }
    }
}