using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BASRemote.Common;
using BASRemote.Objects;
using BASRemote.Promises;

namespace BASRemote
{
    /// <summary>
    ///     Provides methods for interact with BAS.
    /// </summary>
    public interface IBasRemoteClient : IDisposable
    {
        /// <summary>
        ///     List of all running BAS browsers.
        /// </summary>
        IReadOnlyDictionary<int, IBasBrowser> Browsers { get; }

        /// <summary>
        ///     List of all running BAS tasks.
        /// </summary>
        IReadOnlyDictionary<int, IBasTask> Tasks { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        IPromise<string> OpenFileDialog(FileDialogOptions options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        IPromise<string> SaveFileDialog(FileDialogOptions options);

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        IPromise SetGlobalVariable(string name, dynamic value);

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        IPromise<dynamic> GetGlobalVariable(string name);

        /// <summary>
        ///     Obtains resources report text.
        /// </summary>
        IPromise<string> GetResourcesReport();

        /// <summary>
        ///     Obtains script report text.
        /// </summary>
        IPromise<string> GetScriptReport();

        /// <summary>
        ///     Obtains result file content.
        /// </summary>
        /// <param name="number">
        ///     Result number index.
        /// </param>
        IPromise<string> DownloadResult(int number);

        /// <summary>
        ///     Obtains log file content.
        /// </summary>
        IPromise<string> DownloadLog();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="browserId"></param>
        void ShowBrowser(int browserId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="browserId"></param>
        void HideBrowser(int browserId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IBasThread CreateThread();

        /// <summary>
        /// 
        /// </summary>
        void InstallScheduler();

        /// <summary>
        /// 
        /// </summary>
        void ShowScheduler();

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="data"></param>
        Task<T> SendAndWaitAsync<T>(string type, Params data = null);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        Task SendAndWaitAsync(string type, Params data = null);

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="data"></param>
        IPromise<T> SendAsync<T>(string type, Params data = null);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        IPromise SendAsync(string type, Params data = null);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="async"></param>
        int Send(string type, Params data, bool async = false);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="async"></param>
        int Send(string type, bool async = false);

        /// <summary>
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// </summary>
        Task StopAsync();

        void AddTask(int id, string type, IBasThread basThread, string functionName, Params functionParams);

        void RemoveTask(int id);
    }
}