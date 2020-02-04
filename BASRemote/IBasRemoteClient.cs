using System;
using System.Threading.Tasks;
using BASRemote.Objects;

namespace BASRemote
{
    /// <summary>
    ///     Provides methods for interact with BAS.
    /// </summary>
    public interface IBasRemoteClient : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        event Action<string, dynamic> OnMessage;

        /// <summary>
        /// 
        /// </summary>
        event Action OnEngineDownloadStarted;

        /// <summary>
        /// 
        /// </summary>
        event Action OnEngineExtractStarted;

        /// <summary>
        /// 
        /// </summary>
        event Action OnEngineDownloadEnded;

        /// <summary>
        /// 
        /// </summary>
        event Action OnEngineExtractEnded;

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        Task<dynamic> SendAndWaitAsync(string type, Params data = null);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        Task<T> SendAndWaitAsync<T>(string type, Params data = null);

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="onResult"></param>
        void SendAsync(string type, Params data, Action<dynamic> onResult);

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="onResult"></param>
        void SendAsync<T>(string type, Params data, Action<T> onResult);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="onResult"></param>
        void SendAsync(string type, Params data, Action onResult);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        void SendAsync(string type, Params data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="onResult"></param>
        void SendAsync(string type, Action<dynamic> onResult);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="onResult"></param>
        void SendAsync<T>(string type, Action<T> onResult);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="onResult"></param>
        void SendAsync(string type, Action onResult);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        void SendAsync(string type);

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
        /// 
        /// </summary>
        IBasThread CreateThread();

        /// <summary>
        /// </summary>
        Task StartAsync();
    }
}