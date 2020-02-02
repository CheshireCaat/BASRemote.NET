using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BASRemote.Common;
using BASRemote.Objects;

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
        /// <param name="browserId"></param>
        /// <param name="taskId"></param>
        void AddBrowser(int browserId, int taskId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="browserId"></param>
        void RemoveBrowser(int browserId);

        Task<T> SendAndWaitAsync<T>(string type, Params data);

        Task<T> SendAndWaitAsync<T>(string type);

        IPromise<T> SendAsync<T>(string type, Params data);

        IPromise<T> SendAsync<T>(string type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="async"></param>
        int Send(string type, Params data, bool async = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="async"></param>
        /// <returns></returns>
        int Send(string type, bool async = false);

        /// <summary>
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// 
        /// </summary>
        Task StopAsync();

        void AddTask(int id, string type, IBasThread basThread, string functionName, Params functionParams);

        void RemoveTask(int id);
    }
}