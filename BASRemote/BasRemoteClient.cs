using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BASRemote.Helpers;
using BASRemote.Interfaces;
using BASRemote.Objects;
using BASRemote.Services;

namespace BASRemote
{
    /// <inheritdoc cref="IBasRemoteClient" />
    public sealed class BasRemoteClient : IBasRemoteClient, IFunctionRunner<IBasFunction>
    {
        private readonly ConcurrentDictionary<int, object> _requests = new ConcurrentDictionary<int, object>();

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0, 1);

        private EngineService _engine;

        private SocketService _socket;

        /// <summary>
        ///     Create an instance of <see cref="BasRemoteClient" /> class.
        /// </summary>
        /// <param name="options">
        ///     Remote control options.
        /// </param>
        public BasRemoteClient(Options options)
        {
            _engine = new EngineService(options);
            _socket = new SocketService(options);

            _engine.OnDownloadStarted += () => OnEngineDownloadStarted?.Invoke();
            _engine.OnDownloadEnded += () => OnEngineDownloadEnded?.Invoke();

            _engine.OnExtractStarted += () => OnEngineExtractStarted?.Invoke();
            _engine.OnExtractEnded += () => OnEngineExtractEnded?.Invoke();

            _socket.OnMessage += message =>
            {
                if (message.Type == "thread_start")
                {
                    if (_semaphore.CurrentCount == 0)
                        _semaphore.Release();
                }

                if (message.Type == "initialize")
                {
                    Send("accept_resources", new Params
                    {
                        {"-bas-empty-script-", true}
                    });
                }
                else if (message.Async && message.Id != 0)
                {
                    if (_requests.TryRemove(message.Id, out var function))
                    {
                        if (function.GetType().IsGenericType)
                        {
                            (function as dynamic)(message.Data);
                        }
                        else
                        {
                            (function as dynamic)();
                        }
                    }
                }
                else
                {
                    OnMessage?.Invoke(message.Type, message.Data);
                }
            };
        }

        /// <inheritdoc />
        public event Action<string, dynamic> OnMessage;

        /// <inheritdoc />
        public event Action OnEngineDownloadStarted;

        /// <inheritdoc />
        public event Action OnEngineExtractStarted;

        /// <inheritdoc />
        public event Action OnEngineDownloadEnded;

        /// <inheritdoc />
        public event Action OnEngineExtractEnded;

        /// <inheritdoc />
        public async Task StartAsync()
        {
            await _engine.InitializeAsync().ConfigureAwait(false);

            var port = Rand.NextInt(10000, 20000);

            await _engine.StartEngineAsync(port)
                .ConfigureAwait(false);
            await _socket.StartSocketAsync(port)
                .ConfigureAwait(false);

            await _semaphore.WaitAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<dynamic> SendAndWaitAsync(string type, Params data = null)
        {
            return await SendAndWaitAsync<dynamic>(type, data);
        }

        /// <inheritdoc />
        public async Task<T> SendAndWaitAsync<T>(string type, Params data = null)
        {
            var tcs = new TaskCompletionSource<T>();
            SendAsync<T>(type, data, result => tcs.TrySetResult(result));
            return await tcs.Task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void SendAsync(string type, Params data, Action<dynamic> onResult)
        {
            SendAsync<dynamic>(type, data, onResult);
        }

        /// <inheritdoc />
        public void SendAsync<T>(string type, Params data, Action<T> onResult)
        {
            var message = new Message(data ?? Params.Empty, type, true);
            Task.Run(() =>
            {
                _socket.SendAsync(message);
                _requests[message.Id] = onResult;
            });
        }

        /// <inheritdoc />
        public void SendAsync(string type, Params data, Action onResult)
        {
            SendAsync(type, data, result => onResult());
        }

        /// <inheritdoc />
        public void SendAsync(string type, Params data)
        {
            SendAsync(type, data, result => { });
        }

        /// <inheritdoc />
        public void SendAsync(string type, Action<dynamic> onResult)
        {
            SendAsync<dynamic>(type, onResult);
        }

        /// <inheritdoc />
        public void SendAsync<T>(string type, Action<T> onResult)
        {
            var message = new Message(Params.Empty, type, true);
            Task.Run(() =>
            {
                _socket.SendAsync(message);
                _requests[message.Id] = onResult;
            });
        }

        /// <inheritdoc />
        public void SendAsync(string type, Action onResult)
        {
            SendAsync(type, result => onResult());
        }

        /// <inheritdoc />
        public void SendAsync(string type)
        {
            SendAsync(type, result => { });
        }

        /// <inheritdoc />
        public int Send(string type, Params data, bool async = false)
        {
            var message = new Message(data, type, async);
            _socket.SendAsync(message);
            return message.Id;
        }

        /// <inheritdoc />
        public int Send(string type, bool async = false)
        {
            return Send(type, Params.Empty, async);
        }

        /// <inheritdoc />
        public IBasThread CreateThread()
        {
            return new BasThread(this);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _engine?.Dispose();
            _socket?.Dispose();
            _engine = null;
            _socket = null;
        }

        /// <inheritdoc />
        public async Task<dynamic> RunFunction(string functionName, Params functionParams)
        {
            var tcs = new TaskCompletionSource<dynamic>();

            RunFunctionSync(functionName, functionParams,
                result => tcs.TrySetResult(result),
                exception => tcs.TrySetException(exception));

            return await tcs.Task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<T> RunFunction<T>(string functionName, Params functionParams)
        {
            var tcs = new TaskCompletionSource<T>();

            RunFunctionSync(functionName, functionParams,
                result => tcs.TrySetResult((T) result),
                exception => tcs.TrySetException(exception));

            return await tcs.Task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public IBasFunction RunFunctionSync(
            string functionName,
            Params functionParams,
            Action<dynamic> onResult,
            Action<Exception> onError)
        {
            EnsureClientStarted();

            return new BasFunction(this).RunFunctionSync(functionName, functionParams, onResult, onError);
        }

        private void EnsureClientStarted()
        {
            if (_semaphore.CurrentCount == 0)
            {
                throw new ApplicationException();
            }
        }
    }
}