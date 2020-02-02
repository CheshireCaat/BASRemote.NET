using System;
using Newtonsoft.Json;

namespace BASRemote.Objects
{
    internal sealed class Script
    {
        private static readonly Version SupportedVersion = Version.Parse("22.4.2");

        [JsonProperty("engversion")] 
        private Version _engineVersion;

        [JsonProperty("hash")] 
        private string _hash;

        public bool IsSupported => _engineVersion >= SupportedVersion;

        [JsonProperty("success")] 
        public bool IsExist { get; private set; }

        [JsonProperty("free")] 
        public bool IsFree { get; private set; }

        public string EngineVersion => _engineVersion.ToString(3);

        public string Hash => _hash.Substring(0, 5);
    }
}