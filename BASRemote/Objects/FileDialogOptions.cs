using Newtonsoft.Json;

namespace BASRemote.Objects
{
    public sealed class FileDialogOptions
    {
        [JsonProperty("is_dir")] 
        public bool IsDirectory { get; set; }

        [JsonProperty("dir")] 
        public string Directory { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("filter")] 
        public string Filter { get; set; }

        internal Params ToParams()
        {
            return new Params
            {
                {"is_dir", IsDirectory},
                {"caption", Caption},
                {"dir", Directory},
                {"filter", Filter}
            };
        }
    }
}