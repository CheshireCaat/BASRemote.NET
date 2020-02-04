using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.Helpers;
using Newtonsoft.Json;

namespace BASRemote.Services
{
    /// <inheritdoc cref="IEngineService" />
    internal sealed class EngineService : BaseService, IEngineService
    {
        private const string Endpoint = "https://bablosoft.com";

        /// <summary>
        ///     Engine process object.
        /// </summary>
        private Process _process;

        /// <summary>
        ///     Engine script object.
        /// </summary>
        private Script _script;

        /// <summary>
        ///     File lock object.
        /// </summary>
        private FileLock _lock;

        /// <summary>
        ///     Create an instance of <see cref="EngineService" /> class.
        /// </summary>
        public EngineService(Options options) : base(options)
        {
            ScriptDirectory = Path.Combine(Options.WorkingDirectory, "run", Options.ScriptName);
            EngineDirectory = Path.Combine(Options.WorkingDirectory, "engine");
        }

        private string ScriptDirectory { get; }

        private string EngineDirectory { get; }

        /// <summary>
        ///     The path to the directory in which the executable file of the engine is located.
        /// </summary>
        private string ExeDirectory { get; set; }

        /// <summary>
        ///     The path to the directory in which the archive file of the engine is located.
        /// </summary>
        private string ZipDirectory { get; set; }

        /// <inheritdoc />
        public event Action OnExtractStarted;

        /// <inheritdoc />
        public event Action OnExtractEnded;

        /// <inheritdoc />
        public event Action OnDownloadStarted;

        /// <inheritdoc />
        public event Action OnDownloadEnded;

        /// <inheritdoc />
        public async Task StartEngineAsync(int port)
        {
            var version = Environment.Is64BitOperatingSystem ? 64 : 32;
            var zipName = $"FastExecuteScriptProtected.x{version}";
            var urlName = $"FastExecuteScriptProtected{version}";

            var zipPath = Path.Combine(ZipDirectory, $"{zipName}.zip");

            if (!Directory.Exists(ZipDirectory))
            {
                Directory.CreateDirectory(ZipDirectory);
                await DownloadExecutable(zipPath, zipName, urlName)
                    .ConfigureAwait(false);
            }

            if (!Directory.Exists(ExeDirectory))
            {
                Directory.CreateDirectory(ExeDirectory);
                await ExtractExecutable(zipPath)
                    .ConfigureAwait(false);
            }

            StartEngineProcess(port);
            ClearRunDirectory();
        }

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            var url = $"{Endpoint}/scripts/{Options.ScriptName}/properties";

            using (var client = new WebClient())
            {
                var response = await client.DownloadStringTaskAsync(url).ConfigureAwait(false);
                _script = JsonConvert.DeserializeObject<Script>(response);

                if (!_script.IsSupported)
                {
                    throw new ScriptNotSupportedException();
                }

                if (!_script.IsExist)
                {
                    throw new ScriptNotExistException();
                }

                ZipDirectory = Path.Combine(EngineDirectory, _script.EngineVersion);
                ExeDirectory = Path.Combine(ScriptDirectory, _script.Hash.Substring(0, 5));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="zipPath"></param>
        /// <param name="zipName"></param>
        /// <param name="urlName"></param>
        private async Task DownloadExecutable(string zipPath, string zipName, string urlName)
        {
            var url = $"{Endpoint}/distr/{urlName}/{Path.GetFileName(ZipDirectory)}/{zipName}.zip";

            using (var client = new WebClient())
            {
                OnDownloadStarted?.Invoke();
                
                await client.DownloadFileTaskAsync(url, zipPath).ConfigureAwait(false);

                OnDownloadEnded?.Invoke();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="zipPath"></param>
        /// <returns></returns>
        private async Task ExtractExecutable(string zipPath)
        {
            await Task.Run(() => ZipFile.ExtractToDirectory(zipPath, ExeDirectory));
        }

        private void StartEngineProcess(int port)
        {
            _process = Process.Start
            (
                new ProcessStartInfo
                {
                    Arguments = $"--remote-control --remote-control-port={port}",
                    FileName = Path.Combine(ExeDirectory, "FastExecuteScript.exe"),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            );

            _lock = new FileLock(Path.Combine(ExeDirectory, ".lock"));
        }

        private void ClearRunDirectory()
        {
            foreach (var directory in Directory.GetDirectories(ScriptDirectory))
            {
                var lockFile = Path.Combine(directory, ".lock");
                if (!FileLock.IsLocked(lockFile))
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

        public void Dispose()
        {
            _process?.Dispose();
            _lock?.Dispose();
            _process = null;
            _lock = null;
        }

        private sealed class Script
        {
            [JsonProperty("engversion")] public string EngineVersion { get; private set; }

            public bool IsSupported => Version.Parse(EngineVersion) >= Version.Parse("22.4.2");

            [JsonProperty("success")] public bool IsExist { get; private set; }

            [JsonProperty("free")] public bool IsFree { get; private set; }

            [JsonProperty("hash")] public string Hash { get; private set; }
        }
    }
}