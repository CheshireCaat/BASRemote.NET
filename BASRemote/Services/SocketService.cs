using System;
using System.Diagnostics;
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
    ///     Provides methods for interacting with a BAS socket.
    /// </summary>
    internal sealed class SocketService : BaseService
    {
        private WebSocket _socket;

        private string _buffer;

        private int _tries;

        /// <summary>
        ///     Create an instance of <see cref="SocketService" /> class.
        /// </summary>
        public SocketService(Options options) : base(options)
        {
        }

        /// <summary>
        ///     Occurs when <see cref="SocketService"/> receives a message.
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
        public async Task StartSocketAsync(int port)
        {
            _socket = new WebSocket($"ws://127.0.0.1:{port}")
            {
                Log = {Output = (_, __) => { }}
            };

            _socket.OnMessage += (sender, args) =>
            {
                _buffer += args.Data;

                var split = _buffer.Split("---Message--End---");

                foreach (var message in split)
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        OnMessage?.Invoke(message.FromJson<Message>());
                    }
                }

                //Debug.WriteLine($"<-- {args.Data}");

                _buffer = split.Last();
            };

            _socket.OnOpen += (sender, args) =>
            {
                SendAsync("remote_control_data", new Params
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
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="async"></param>
        public void SendAsync(string type, Params data, bool async = false)
        {
            SendAsync(new Message(data, type, async));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void SendAsync(Message message)
        {
            //Debug.WriteLine($"--> {message.ToJson()}");
            _socket.SendAsync($"{message.ToJson()}---Message--End---", b => {});
        }

        public void Dispose()
        {
            _socket?.Close();
            _socket = null;
        }
    }
}