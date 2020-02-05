using System;
using System.Threading.Tasks;
using BASRemote.Interfaces;
using BASRemote.Objects;

namespace BASRemote
{
    /// <summary>
    ///     Provides methods for remotely interacting with BAS.
    /// </summary>
    public interface IBasRemoteClient : IDisposable, IFunctionRunner<IBasFunction>
    {
        /// <summary>
        /// </summary>
        event Action<string, dynamic> OnMessage;

        /// <summary>
        /// </summary>
        event Action OnEngineDownloadStarted;

        /// <summary>
        /// </summary>
        event Action OnEngineExtractStarted;

        /// <summary>
        /// </summary>
        event Action OnEngineDownloadEnded;

        /// <summary>
        /// </summary>
        event Action OnEngineExtractEnded;

        /// <summary>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="type"></param>
        /// <param name="data"></param>
        Task<TResult> SendAndWaitAsync<TResult>(string type, Params data = null);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        Task<dynamic> SendAndWaitAsync(string type, Params data = null);

        /// <summary>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="onResult"></param>
        void SendAsync<TResult>(string type, Params data, Action<TResult> onResult);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="onResult"></param>
        void SendAsync(string type, Params data, Action<dynamic> onResult);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="onResult"></param>
        void SendAsync(string type, Params data, Action onResult);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        void SendAsync(string type, Params data);

        /// <summary>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="type"></param>
        /// <param name="onResult"></param>
        void SendAsync<TResult>(string type, Action<TResult> onResult);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="onResult"></param>
        void SendAsync(string type, Action<dynamic> onResult);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="onResult"></param>
        void SendAsync(string type, Action onResult);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        void SendAsync(string type);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="async"></param>
        int Send(string type, Params data = null, bool async = false);

        /// <summary>
        /// </summary>
        IBasThread CreateThread();

        /// <summary>
        /// </summary>
        Task StartAsync();
    }
}