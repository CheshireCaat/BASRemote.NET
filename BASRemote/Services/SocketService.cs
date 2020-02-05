using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.Extensions;
using BASRemote.Objects;
using WebSocketSharp;

namespace BASRemote.Services
{
    /// <summary>
    ///     Service that provides methods for interacting with a BAS socket.
    /// </summary>
    internal sealed class SocketService : BaseService
    {
        private readonly object _sync = new object();

        private WebSocket _socket;

        private string _buffer;

        private int _tries;

        /// <summary>
        ///     Create an instance of <see cref="SocketService" /> class.
        /// </summary>
        public SocketService(Options options) : base(options)
        {
        }

        private string Buffer
        {
            get
            {
                lock (_sync)
                {
                    return _buffer;
                }
            }
            set
            {
                lock (_sync)
                {
                    _buffer = value;
                }
            }
        }

        /// <summary>
        ///     Occurs when <see cref="SocketService" /> receives a message.
        /// </summary>
        public event Action<Message> OnMessage;

        /// <summary>
        ///     Occurs when <see cref="SocketService" /> connection has been closed.
        /// </summary>
        public event Action OnClose;

        /// <summary>
        ///     Occurs when <see cref="SocketService" /> connection has been opened.
        /// </summary>
        public event Action OnOpen;

        /// <summary>
        ///     Asynchronously start the socket service with the specified port.
        /// </summary>
        /// <param name="port">
        ///     Selected port number.
        /// </param>
        public override async Task StartServiceAsync(int port)
        {
            _socket = new WebSocket($"ws://127.0.0.1:{port}")
            {
                Log = {Output = (_, __) => { }}
            };

            _socket.OnMessage += (sender, args) =>
            {
                Buffer += args.Data;

                var split = Buffer.Split("---Message--End---");

                for (var i = 0; i < split.Length - 1; i++)
                {
                    OnMessage?.Invoke(split[i].FromJson<Message>());
                }

                Buffer = split.Last();
            };

            _socket.OnOpen += (sender, args) =>
            {
                Send("remote_control_data", new Params
                {
                    {"script", Options.ScriptName},
                    {"password", Options.Password},
                    {"login", Options.Login}
                });
            };

            await ConnectAsync().ConfigureAwait(false);
        }

        private async Task ConnectAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            _socket.OnClose += (sender, args) =>
            {
                if (!args.WasClean)
                {
                    if (_tries == 60)
                    {
                        tcs.TrySetException(new SocketNotConnectedException());
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    _socket.Connect();
                    _tries++;
                }

                OnClose?.Invoke();
            };

            _socket.OnOpen += (sender, args) =>
            {
                tcs.TrySetResult(true);
                OnOpen?.Invoke();
            };

            _socket.ConnectAsync();

            await tcs.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="async"></param>
        public void Send(string type, Params data, bool async = false)
        {
            Send(new Message(data, type, async));
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public void Send(Message message)
        {
            _socket.Send($"{message.ToJson()}---Message--End---");
        }

        public void Dispose()
        {
            _socket?.Close();
            _socket = null;
        }
    }
}