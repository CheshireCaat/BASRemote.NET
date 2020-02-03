using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BASRemote.Objects;
using WebSocketSharp;

namespace BASRemote.Services
{
    /// <inheritdoc cref="ISocketService" />
    internal sealed class SocketService : BaseService, ISocketService
    {
        private const string Separator = "---Message--End---";

        private string _buffer;

        private WebSocket _socket;

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

        public event Action<Message> OnMessage;

        public event Action OnClose;

        public event Action OnOpen;

        /// <inheritdoc />
        public async Task StartSocketAsync(int port)
        {
            _socket = new WebSocket($"ws://127.0.0.1:{port}");

            _socket.OnMessage += (sender, args) =>
            {
                _buffer += args.Data;

                var split = _buffer.Split(
                    new[]
                    {
                        Separator
                    }, StringSplitOptions.None);

                foreach (var message in split)
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        OnMessage?.Invoke(Message.FromJson(message));
                    }
                }

                Debug.WriteLine($"<-- {args.Data}");

                _buffer = split.Last();
            };

            _socket.OnClose += (sender, args) => OnClose?.Invoke();
            _socket.OnOpen += (sender, args) => OnOpen?.Invoke();
            await ConnectAsync().ConfigureAwait(false);
        }

        private async Task ConnectAsync()
        {
            for (var i = 0; i < 60; i++)
                if (_socket.ReadyState != WebSocketState.Open)
                {
                    _socket.Connect();
                    await Task.Delay(1000).ConfigureAwait(false);
                }
                else
                {
                    return;
                }

            throw new ApplicationException("bla-bla");
        }

        /// <inheritdoc />
        public void SendAsync(Message message)
        {
            _socket.SendAsync($"{message.ToJson()}{Separator}", b => { });
        }
    }
}