using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace BASRemote.Services
{
    /// <summary>
    ///     Provides methods for interacting with BAS engine.
    /// </summary>
    internal sealed class EngineService : BaseService, IDisposable
    {
        private readonly string _scriptDirectory;

        private readonly string _engineDirectory;

        private readonly string _runDirectory;

        private string _exeDirectory;

        /// <summary>
        /// </summary>
        private FileStream _lock;

        private string _zipDirectory;

        /// <summary>
        ///     Create an instance of <see cref="EngineService" /> class.
        /// </summary>
        /// <param name="options">
        ///     Remote control options.ц
        /// </param>
        public EngineService(BasRemoteOptions options) : base(options)
        {
            _engineDirectory = Path.Combine(Options.WorkingDirectory, "engine");
            _runDirectory = Path.Combine(Options.WorkingDirectory, "run");

            _scriptDirectory = Path.Combine(_runDirectory, Options.ScriptName);
        }

        public void Dispose()
        {
            _lock?.Dispose();
            _lock = null;
        }

        public async Task InitializeAsync()
        {
            //using (var client = new HttpClient())
            //{
            //    var result = await client.GetStringAsync($"https://bablosoft.com/scripts/{Options.ScriptName}/properties");
            //    Script = JsonConvert.DeserializeObject<Script>(result);
            //}

            var script = await "https://bablosoft.com"
                .AppendPathSegment("scripts")
                .AppendPathSegment(Options.ScriptName)
                .AppendPathSegment("properties")
                .GetJsonAsync<Script>()
                .ConfigureAwait(false);

            if (!script.IsSupported) throw new ScriptNotSupportedException();
            if (!script.IsExist) throw new ScriptNotExistException();

            _zipDirectory = Path.Combine(_engineDirectory, script.EngineVersion);
            _exeDirectory = Path.Combine(_scriptDirectory, script.Hash.Substring(0, 5));
        }

        public async Task<string> DownloadExecutable(string zipName, string urlName)
        {
            return await "https://bablosoft.com"
                .AppendPathSegment("distr")
                .AppendPathSegment(urlName)
                .AppendPathSegment(Path.GetDirectoryName(_zipDirectory))
                .AppendPathSegment($"{zipName}.zip")
                .DownloadFileAsync(_zipDirectory, $"{urlName}.zip")
                .ConfigureAwait(false);
        }

        public async Task ExtractExecutable(string zipPath)
        {
            //_extractSubject.OnNext(Unit.Default);

            await Task.Run(() => ZipFile.ExtractToDirectory(zipPath, _exeDirectory));

            //_extractSubject.OnCompleted();
        }

        public async Task StartEngineAsync(int port)
        {
            var version = Environment.Is64BitOperatingSystem ? 64 : 32;
            var zipName = $"FastExecuteScriptProtected.x{version}";
            var urlName = $"FastExecuteScriptProtected{version}";

            var zipPath = Path.Combine(_zipDirectory, $"{urlName}.zip");

            if (!Directory.Exists(_zipDirectory))
                await DownloadExecutable(zipName, urlName)
                    .ConfigureAwait(false);

            if (!Directory.Exists(_exeDirectory))
                await ExtractExecutable(zipPath)
                    .ConfigureAwait(false);

            StartEngineProcess(port);
        }

        private void StartEngineProcess(int port)
        {
            Process.Start
            (
                new ProcessStartInfo
                {
                    Arguments = $"--remote-control --remote-control-port={port}",
                    FileName = Path.Combine(_exeDirectory, "FastExecuteScript.exe"),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            );
        }

        private void ClearRunDirectory()
        {
            foreach (var directory in Directory.GetDirectories(_scriptDirectory))
            {
                var lockFile = Path.Combine(directory, ".lock");
                if (!File.Exists(lockFile))
                {
                    Debug.WriteLine($"Lock file not exist in {directory}");
                    Directory.Delete(directory, true);
                }
                else
                {
                    Debug.WriteLine($"Lock file exist in {directory}");
                }
            }
        }

        private class Script
        {
            [JsonProperty("engversion")] 
            public string EngineVersion { get; private set; }

            public bool IsSupported
            {
                get
                {
                    return Version.Parse(EngineVersion) >= Version.Parse("22.4.2");
                }
            }

            [JsonProperty("success")]
            public bool IsExist { get; private set; }

            [JsonProperty("free")] 
            public bool IsFree { get; private set; }

            [JsonProperty("hash")] 
            public string Hash { get; private set; }
        }
    }
}