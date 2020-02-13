using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.Extensions;
using BASRemote.Helpers;
using BASRemote.Objects;
using BASRemote.Services;

namespace BASRemote
{
    /// <inheritdoc cref="IBasRemoteClient" />
    public sealed class BasRemoteClient : IBasRemoteClient
    {
        /// <summary>
        ///     Dictionary of generic requests handlers.
        /// </summary>
        private readonly ConcurrentDictionary<int, Action<object>> _genericRequests = new ConcurrentDictionary<int, Action<object>>();

        /// <summary>
        ///     Dictionary of default requests handlers.
        /// </summary>
        private readonly ConcurrentDictionary<int, Action> _defaultRequests = new ConcurrentDictionary<int, Action>();

        /// <summary>
        /// 
        /// </summary>
        private readonly TaskCompletionSource<bool> _completion = new TaskCompletionSource<bool>();

        /// <summary>
        ///     Engine service provider object.
        /// </summary>
        private EngineService _engine;

        /// <summary>
        ///     Socket service provider object.
        /// </summary>
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
            _engine.OnExtractStarted += () => OnEngineExtractStarted?.Invoke();

            _engine.OnDownloadEnded += () => OnEngineDownloadEnded?.Invoke();
            _engine.OnExtractEnded += () => OnEngineExtractEnded?.Invoke();

            _socket.OnMessageReceived += message =>
            {
                OnMessageReceived?.Invoke(message.Type, message.Data);

                if (message.Type == "message")
                {
                    _completion.TrySetException(new AuthenticationException((string) message.Data["text"]));
                }

                if (message.Type == "thread_start")
                {
                    _completion.TrySetResult(true);
                }

                if (message.Type == "initialize")
                {
                    _socket.Send("accept_resources", new Params
                    {
                        {"-bas-empty-script-", true}
                    });
                }
                else if (message.Async && message.Id != 0)
                {
                    if (_genericRequests.TryRemove(message.Id, out var genericFunction))
                    {
                        (genericFunction as dynamic)(message.Data);
                    }

                    if (_defaultRequests.TryRemove(message.Id, out var defaultFunction))
                    {
                        (defaultFunction as dynamic)();
                    }
                }
            };

            _socket.OnMessageSent += message => OnMessageSent?.Invoke(message);
        }

        /// <inheritdoc />
        public event Action<string, dynamic> OnMessageReceived;

        /// <inheritdoc />
        public event Action<Message> OnMessageSent;

        /// <inheritdoc />
        public event Action OnEngineDownloadStarted;

        /// <inheritdoc />
        public event Action OnEngineExtractStarted;

        /// <inheritdoc />
        public event Action OnEngineDownloadEnded;

        /// <inheritdoc />
        public event Action OnEngineExtractEnded;

        /// <inheritdoc />
        public async Task Start()
        {
            await _engine.InitializeAsync().ConfigureAwait(false);

            var port = Rand.NextInt(10000, 20000);

            await _engine.StartServiceAsync(port).ConfigureAwait(false);
            await _socket.StartServiceAsync(port).ConfigureAwait(false);

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60)))
            {
                using (cts.Token.Register(() => _completion.TrySetCanceled()))
                {
                    await _completion.Task.ConfigureAwait(false);
                }
            }
        }

        /// <inheritdoc />
        public IBasFunction RunFunction(string functionName, Params functionParams)
        {
            EnsureClientStarted();

            var functionObj = new BasFunction(this);
            return functionObj.RunFunction(functionName, functionParams);
        }

        /// <inheritdoc />
        public IBasFunction RunFunction(string functionName)
        {
            return RunFunction(functionName, Params.Empty);
        }

        /// <inheritdoc />
        public async Task<TResult> SendAndWaitAsync<TResult>(string type, Params data = null)
        {
            var tcs = new TaskCompletionSource<TResult>();
            SendAsync<TResult>(type, data, result => tcs.TrySetResult(result));
            return await tcs.Task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<dynamic> SendAndWaitAsync(string type, Params data = null)
        {
            var tcs = new TaskCompletionSource<dynamic>();
            SendAsync(type, data, result => tcs.TrySetResult(result));
            return await tcs.Task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void SendAsync<TResult>(string type, Params data, Action<TResult> onResult)
        {
            EnsureClientStarted();

            var message = new Message(data ?? Params.Empty, type, true);
            _genericRequests.TryAdd(message.Id, obj => onResult(obj.Convert<TResult>()));
            _socket.Send(message);
        }

        /// <inheritdoc />
        public void SendAsync(string type, Params data, Action<dynamic> onResult)
        {
            EnsureClientStarted();

            var message = new Message(data ?? Params.Empty, type, true);
            _genericRequests.TryAdd(message.Id, onResult);
            _socket.Send(message);
        }

        /// <inheritdoc />
        public void SendAsync(string type, Params data, Action onResult)
        {
            EnsureClientStarted();

            var message = new Message(data ?? Params.Empty, type, true);
            _defaultRequests.TryAdd(message.Id, onResult);
            _socket.Send(message);
        }

        /// <inheritdoc />
        public void SendAsync(string type, Params data)
        {
            SendAsync(type, data, () => { });
        }

        /// <inheritdoc />
        public void SendAsync<TResult>(string type, Action<TResult> onResult)
        {
            SendAsync(type, Params.Empty, onResult);
        }

        /// <inheritdoc />
        public void SendAsync(string type, Action<dynamic> onResult)
        {
            SendAsync(type, Params.Empty, onResult);
        }

        /// <inheritdoc />
        public void SendAsync(string type, Action onResult)
        {
            SendAsync(type, Params.Empty, onResult);
        }

        /// <inheritdoc />
        public void SendAsync(string type)
        {
            SendAsync(type, () => { });
        }

        /// <inheritdoc />
        public int Send(string type, Params data = null, bool async = false)
        {
            EnsureClientStarted();

            var message = new Message(data ?? Params.Empty, type, async);
            _socket.Send(message);
            return message.Id;
        }

        private void EnsureClientStarted()
        {
            if (!_completion.Task.IsCompleted)
            {
                throw new ClientNotStartedException();
            }
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
    }
}