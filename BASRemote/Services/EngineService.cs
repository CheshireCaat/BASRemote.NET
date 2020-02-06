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
    /// <summary>
    ///     Service that provides methods for interacting with BAS engine.
    /// </summary>
    internal sealed class EngineService : BaseService
    {
        private const string Endpoint = "https://bablosoft.com";

        /// <summary>
        ///     Engine process object.
        /// </summary>
        private Process _process;

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

        /// <summary>
        ///     The path to the directory in which all versions of the script are located.
        /// </summary>
        private string ScriptDirectory { get; }

        /// <summary>
        ///     The path to the directory in which all versions of the engine are located.
        /// </summary>
        private string EngineDirectory { get; }

        /// <summary>
        ///     The path to the directory in which the executable file of the engine is located.
        /// </summary>
        private string ExeDirectory { get; set; }

        /// <summary>
        ///     The path to the directory in which the archive file of the engine is located.
        /// </summary>
        private string ZipDirectory { get; set; }

        /// <summary>
        ///     Occurs when <see cref="EngineService" /> starts downloading executable file.
        /// </summary>
        public event Action OnDownloadStarted;

        /// <summary>
        ///     Occurs when <see cref="EngineService" /> starts extracting executable file.
        /// </summary>
        public event Action OnExtractStarted;

        /// <summary>
        ///     Occurs when <see cref="EngineService" /> ends downloading executable file.
        /// </summary>
        public event Action OnDownloadEnded;

        /// <summary>
        ///     Occurs when <see cref="EngineService" /> ends extracting executable file.
        /// </summary>
        public event Action OnExtractEnded;

        /// <summary>
        ///     Asynchronously start the engine service with the specified port.
        /// </summary>
        /// <param name="port">
        ///     Selected port number.
        /// </param>
        public override async Task StartServiceAsync(int port)
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

        /// <summary>
        /// 
        /// </summary>
        public async Task InitializeAsync()
        {
            var url = $"{Endpoint}/scripts/{Options.ScriptName}/properties";

            using (var client = new WebClient())
            {
                var response = await client.DownloadStringTaskAsync(url).ConfigureAwait(false);
                var script = JsonConvert.DeserializeObject<Script>(response);

                if (!script.IsSupported)
                {
                    throw new ScriptNotSupportedException();
                }

                if (!script.IsExist)
                {
                    throw new ScriptNotExistException();
                }

                ZipDirectory = Path.Combine(EngineDirectory, script.EngineVersion);
                ExeDirectory = Path.Combine(ScriptDirectory, script.Hash.Substring(0, 5));
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
        private async Task ExtractExecutable(string zipPath)
        {
            OnExtractStarted?.Invoke();

            using (var zip = new FileStream(zipPath, FileMode.Open))
            {
                using (var archive = new ZipArchive(zip, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        var path = Path.Combine(ExeDirectory, entry.FullName);

                        if (!entry.FullName.EndsWith("/") || !string.IsNullOrEmpty(entry.Name))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());

                            using (Stream stream = entry.Open(), file = File.Open(path, FileMode.Create, FileAccess.Write))
                            {
                                await stream.CopyToAsync(file).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(path);
                        }
                    }
                }
            }

            OnExtractEnded?.Invoke();
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
                if (!FileLock.IsLocked(Path.Combine(directory, ".lock")))
                {
                    Directory.Delete(directory, true);
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