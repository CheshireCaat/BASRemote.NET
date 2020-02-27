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
        private readonly TaskCompletionSource<bool> _completion = new TaskCompletionSource<bool>();

        /// <summary>
        ///     Dictionary of generic requests handlers.
        /// </summary>
        private readonly ConcurrentDictionary<int, Action<object>> _genericRequests =
            new ConcurrentDictionary<int, Action<object>>();

        /// <summary>
        ///     Dictionary of default requests handlers.
        /// </summary>
        private readonly ConcurrentDictionary<int, Action> _defaultRequests =
            new ConcurrentDictionary<int, Action>();

        /// <summary>
        ///     Client engine service object.
        /// </summary>
        private EngineService _engine;

        /// <summary>
        ///     Client socket service object.
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

            _socket.OnMessageReceived += message =>
            {
                switch (message.Type)
                {
                    case "message":
                        _completion.TrySetException(new AuthenticationException((string) message.Data["text"]));
                        break;
                    case "thread_start":
                        _completion.TrySetResult(true);
                        break;
                    case "initialize":
                        _socket.Send("accept_resources", new Params
                        {
                            {"-bas-empty-script-", true}
                        });
                        break;
                    default:
                    {
                        if (message.Async && message.Id != 0)
                        {
                            if (_genericRequests.TryRemove(message.Id, out var genericFunction))
                            {
                                genericFunction(message.Data);
                            }

                            if (_defaultRequests.TryRemove(message.Id, out var defaultFunction))
                            {
                                defaultFunction();
                            }
                        }

                        break;
                    }
                }
            };
        }

        /// <inheritdoc />
        public event Action<Message> OnMessageReceived
        {
            add => _socket.OnMessageReceived += value;
            remove => _socket.OnMessageReceived -= value;
        }

        /// <inheritdoc />
        public event Action<Message> OnMessageSent
        {
            add => _socket.OnMessageSent += value;
            remove => _socket.OnMessageSent -= value;
        }

        /// <inheritdoc />
        public event Action OnEngineDownloadStarted
        {
            add => _engine.OnDownloadStarted += value;
            remove => _engine.OnDownloadStarted -= value;
        }

        /// <inheritdoc />
        public event Action OnEngineExtractStarted
        {
            add => _engine.OnExtractStarted += value;
            remove => _engine.OnExtractStarted -= value;
        }

        /// <inheritdoc />
        public event Action OnEngineDownloadEnded
        {
            add => _engine.OnDownloadEnded += value;
            remove => _engine.OnDownloadEnded -= value;
        }

        /// <inheritdoc />
        public event Action OnEngineExtractEnded
        {
            add => _engine.OnExtractEnded += value;
            remove => _engine.OnExtractEnded -= value;
        }

        /// <inheritdoc />
        public bool IsStarted => _completion.Task.IsCompleted;

        /// <inheritdoc />
        public async Task Start()
        {
            await _engine.InitializeAsync().ConfigureAwait(false);

            var port = Rand.NextInt(10000, 20000);

            await _engine.StartAsync(port).ConfigureAwait(false);
            await _socket.StartAsync(port).ConfigureAwait(false);

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

        private void EnsureClientStarted()
        {
            if (!IsStarted) throw new ClientNotStartedException();
        }
    }
}