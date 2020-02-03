using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.Objects;
using WebSocketSharp;

namespace BASRemote.Services
{
    /// <inheritdoc cref="ISocketService" />
    internal sealed class SocketService : BaseService, ISocketService
    {
        private string _buffer;

        private WebSocket _socket;
        private int _tries;

        /// <summary>
        ///     Create an instance of <see cref="SocketService" /> class.
        /// </summary>
        /// <param name="options">
        ///     Remote control options.
        /// </param>
        public SocketService(BasRemoteOptions options) : base(options)
        {
        }

        public void Dispose()
        {
            _socket?.Close();
            _socket = null;
        }

        /// <inheritdoc />
        public event Action<Message> OnMessage;

        /// <inheritdoc />
        public event Action OnClose;

        /// <inheritdoc />
        public event Action OnOpen;

        /// <inheritdoc />
        public async Task StartSocketAsync(int port)
        {
            _socket = new WebSocket($"ws://127.0.0.1:{port}")
            {
                Log = {Output = (_, __) => { }}
            };

            _socket.OnMessage += (sender, args) =>
            {
                _buffer += args.Data;

                var split = _buffer.Split(new[] {"---Message--End---"}, StringSplitOptions.None);

                foreach (var message in split)
                    if (!string.IsNullOrEmpty(message))
                        OnMessage?.Invoke(Message.FromJson(message));

                Debug.WriteLine($"<-- {args.Data}");

                _buffer = split.Last();
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

                    Thread.Sleep(1000);
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

        /// <inheritdoc />
        public void SendAsync(Message message)
        {
            _socket.SendAsync($"{message.ToJson()}---Message--End---", b => { });
        }
    }
}