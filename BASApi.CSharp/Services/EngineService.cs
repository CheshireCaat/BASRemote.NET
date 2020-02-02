using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.Helpers;
using BASRemote.Objects;
using Flurl;
using Flurl.Http;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;

namespace BASRemote.Services
{
    /// <summary>
    ///     Provides methods for interacting with BAS engine.
    /// </summary>
    internal sealed class EngineService : BaseService, IDisposable
    {
        private readonly Subject<Unit> _downloadSubject = new Subject<Unit>();

        private readonly Subject<Unit> _extractSubject = new Subject<Unit>();

        private FileStream _lock;

        /// <summary>
        ///     Create an instance of <see cref="EngineService" /> class.
        /// </summary>
        /// <param name="options">
        ///     Remote control options.
        /// </param>
        public EngineService(BasRemoteOptions options) : base(options)
        {
        }

        public Script Script { get; private set; }

        private string EngineDirectory => Path.Combine(Options.WorkingDirectory, "engine", Script.EngineVersion);

        private string RunDirectory => Path.Combine(Options.WorkingDirectory, "run", Options.ScriptName);

        private string ExeDirectory => Path.Combine(RunDirectory, Script.Hash);

        public void Dispose()
        {
            _lock?.Dispose();
            _lock = null;
        }

        public async Task InitializeAsync()
        {
            Script = await "https://bablosoft.com"
                .AppendPathSegment("scripts")
                .AppendPathSegment(Options.ScriptName)
                .AppendPathSegment("properties")
                .GetJsonAsync<Script>()
                .ConfigureAwait(false);

            //using (var client = new HttpClient())
            //{
            //    var result = await client.GetStringAsync($"https://bablosoft.com/scripts/{Options.ScriptName}/properties");
            //    Script = JsonConvert.DeserializeObject<Script>(result);
            //}
            if (!Script.IsSupported) throw new ScriptNotSupportedException();
            if (!Script.IsExist) throw new ScriptNotFoundException();
        }

        public async Task<string> DownloadExecutable(string zipName, string urlName)
        {
            return await "https://bablosoft.com"
                .AppendPathSegment("distr")
                .AppendPathSegment(urlName)
                .AppendPathSegment(Script.EngineVersion)
                .AppendPathSegment($"{zipName}.zip")
                .DownloadFileAsync(EngineDirectory, $"{urlName}.zip");
        }

        public async Task ExtractExecutable(string zipPath)
        {
            _extractSubject.OnNext(Unit.Default);

            using (var archive = ZipArchive.Open(zipPath))
            {
                await Task.Run(() => archive.WriteToDirectory(ExeDirectory,
                    new ExtractionOptions
                    {
                        ExtractFullPath = true
                    }));
            }

            _extractSubject.OnCompleted();
        }

        public async Task StartEngineAsync(int port)
        {
            var bitDepth = Environment.Is64BitOperatingSystem ? 64 : 32;
            var zipName = $"FastExecuteScriptProtected.x{bitDepth}";
            var urlName = $"FastExecuteScriptProtected{bitDepth}";

            var zipPath = Path.Combine(EngineDirectory, $"{urlName}.zip");

            if (!Directory.Exists(EngineDirectory))
                await DownloadExecutable(zipName, urlName)
                    .ConfigureAwait(false);

            if (!Directory.Exists(ExeDirectory))
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
                    FileName = Path.Combine(ExeDirectory, "FastExecuteScript.exe"),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            );
        }

        private void ClearRunDirectory()
        {
            foreach (var directory in Directory.GetDirectories(RunDirectory))
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
    }
}