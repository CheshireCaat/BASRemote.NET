using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace BASApi.CSharp.Services
{
    /// <summary>
    /// </summary>
    internal sealed class EngineService : BaseService
    {
        private const string Data = "FastExecuteScriptProtected";

        /// <summary>
        /// </summary>
        /// <param name="options"></param>
        public EngineService(BasRemoteOptions options) : base(options)
        {
        }

        private static int OsNumber => Environment.Is64BitOperatingSystem ? 64 : 32;

        private static string EngineFileName => $"{Data}.x{OsNumber}.zip";

        private static string EnginePathName => $"{Data}{OsNumber}";

        public bool IsSupported => Version.Parse(ScriptVersion) >= Version.Parse("22.4.2");

        public string ScriptVersion { get; private set; }

        public string ScriptHash { get; private set; }

        public async Task Initialize()
        {
            var result = await "https://bablosoft.com"
                .AppendPathSegment("scripts")
                .AppendPathSegment(Options.ScriptName)
                .AppendPathSegment("properties")
                .GetJsonAsync<ScriptProperties>();

            ScriptVersion = result.ScriptVersion;
            ScriptHash = result.ScriptHash;
        }

        public async Task<string> DownloadExecutable(string folder)
        {
            return await "https://bablosoft.com"
                .AppendPathSegment("distr")
                .AppendPathSegment(EnginePathName)
                .AppendPathSegment(ScriptVersion)
                .AppendPathSegment(EngineFileName)
                .DownloadFileAsync(folder);
        }

        public async Task GetExecutable()
        {
            var path = Path.Combine(Options.WorkingDir, "engine", ScriptVersion, EngineFileName);

            if (!File.Exists(path)) await DownloadExecutable(Path.GetDirectoryName(path));

            var destinationPath = Path.Combine(
                Options.WorkingDir, "run",
                Options.ScriptName,
                ScriptHash.Substring(0, 5));

            if (Directory.Exists(destinationPath)) return;

            ZipFile.ExtractToDirectory(path, destinationPath);
        }


        public void RunExecutable(string path, int port)
        {
        }

        private struct ScriptProperties
        {
            [JsonProperty("engversion")] public string ScriptVersion { get; set; }

            [JsonProperty("hash")] public string ScriptHash { get; set; }
        }
    }
}