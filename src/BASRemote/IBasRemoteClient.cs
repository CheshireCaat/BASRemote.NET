using System;
using System.Threading.Tasks;
using BASRemote.Objects;

namespace BASRemote
{
    /// <summary>
    ///     Provides methods for remotely interacting with BAS.
    /// </summary>
    public interface IBasRemoteClient : IDisposable, IFunctionRunner<IBasFunction>
    {
        /// <summary>
        ///     Occurs when client receives any event message from script.
        /// </summary>
        event Action<Message> OnMessageReceived;

        /// <summary>
        ///     Occurs when client sends any event message to script.
        /// </summary>
        event Action<Message> OnMessageSent;

        /// <summary>
        ///     Occurs when client starts downloading executable file.
        /// </summary>
        event Action OnEngineDownloadStarted;

        /// <summary>
        ///     Occurs when client starts extracting executable file.
        /// </summary>
        event Action OnEngineExtractStarted;

        /// <summary>
        ///     Occurs when client ends downloading executable file.
        /// </summary>
        event Action OnEngineDownloadEnded;

        /// <summary>
        ///     Occurs when client ens extracting executable file.
        /// </summary>
        event Action OnEngineExtractEnded;

        /// <summary>
        ///     Send the custom message asynchronously and get result.
        /// </summary>
        /// <typeparam name="TResult">
        ///     Selected result type.
        /// </typeparam>
        /// <param name="type">
        ///     Selected message type.
        /// </param>
        /// <param name="data">
        ///     Message arguments.
        /// </param>
        Task<TResult> SendAndWaitAsync<TResult>(string type, Params data = null);

        /// <summary>
        ///     Send the custom message asynchronously and get result.
        /// </summary>
        /// <param name="type">
        ///     Selected message type.
        /// </param>
        /// <param name="data">
        ///     Message arguments.
        /// </param>
        Task<dynamic> SendAndWaitAsync(string type, Params data = null);

        /// <summary>
        ///     Send the custom message asynchronously and perform action on result.
        /// </summary>
        /// <typeparam name="TResult">
        ///     Selected result type.
        /// </typeparam>
        /// <param name="type">
        ///     Selected message type.
        /// </param>
        /// <param name="data">
        ///     Message arguments.
        /// </param>
        /// <param name="onResult">
        ///     Action that will be performed after receiving the result.
        /// </param>
        void SendAsync<TResult>(string type, Params data, Action<TResult> onResult);

        /// <summary>
        ///     Send the custom message asynchronously and perform action on result.
        /// </summary>
        /// <param name="type">
        ///     Selected message type.
        /// </param>
        /// <param name="data">
        ///     Message arguments.
        /// </param>
        /// <param name="onResult">
        ///     Action that will be performed after receiving the result.
        /// </param>
        void SendAsync(string type, Params data, Action<dynamic> onResult);

        /// <summary>
        ///     Send the custom message asynchronously and perform action on result.
        /// </summary>
        /// <param name="type">
        ///     Selected message type.
        /// </param>
        /// <param name="data">
        ///     Message arguments.
        /// </param>
        /// <param name="onResult">
        ///     Action that will be performed after receiving the result.
        /// </param>
        void SendAsync(string type, Params data, Action onResult);

        /// <summary>
        ///     Send the custom message asynchronously.
        /// </summary>
        /// <param name="type">
        ///     Selected message type.
        /// </param>
        /// <param name="data">
        ///     Message arguments.
        /// </param>
        void SendAsync(string type, Params data);

        /// <summary>
        ///     Send the custom message asynchronously and perform action on result.
        /// </summary>
        /// <param name="type">
        ///     Selected message type.
        /// </param>
        /// <param name="onResult">
        ///     Action that will be performed after receiving the result.
        /// </param>
        void SendAsync<TResult>(string type, Action<TResult> onResult);

        /// <summary>
        ///     Send the custom message asynchronously and perform action on result.
        /// </summary>
        /// <param name="type">
        ///     Selected message type.
        /// </param>
        /// <param name="onResult">
        ///     Action that will be performed after receiving the result.
        /// </param>
        void SendAsync(string type, Action<dynamic> onResult);

        /// <summary>
        ///     Send the custom message asynchronously.
        /// </summary>
        /// <param name="type">
        ///     Selected message type.
        /// </param>
        /// <param name="onResult">
        ///     Action that will be performed after receiving the result.
        /// </param>
        void SendAsync(string type, Action onResult);

        /// <summary>
        ///     Send the custom message asynchronously.
        /// </summary>
        /// <param name="type">
        ///     Selected message type.
        /// </param>
        void SendAsync(string type);

        /// <summary>
        ///     Send the custom message and get message id as result.
        /// </summary>
        /// <param name="type">
        ///     Selected message type.
        /// </param>
        /// <param name="data">
        ///     Message arguments.
        /// </param>
        /// <param name="async">
        ///     Is message async.
        /// </param>
        int Send(string type, Params data = null, bool async = false);

        /// <summary>
        ///     Create new BAS thread object.
        /// </summary>
        IBasThread CreateThread();

        /// <summary>
        ///     Start the client and wait for it initialize.
        /// </summary>
        Task Start();
    }
}