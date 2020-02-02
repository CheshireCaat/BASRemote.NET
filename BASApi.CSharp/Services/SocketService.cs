using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BASRemote.Extensions;
using BASRemote.Helpers;
using BASRemote.Objects;
using Newtonsoft.Json;
using WebSocketSharp;


namespace BASRemote.Services
{
    /// <summary>
    ///     Provides methods for interacting with a BAS socket.
    /// </summary>
    internal sealed class SocketService : BaseService, IDisposable
    {
        private const string Separator = "---Message--End---";

        private WebSocket _socket;

        private string _buffer;

        /// <summary>
        ///     Create an instance of <see cref="SocketService" /> class.
        /// </summary>
        /// <param name="options">
        ///     Remote control options.
        /// </param>
        public SocketService(BasRemoteOptions options) : base(options)
        {
        }

        public event Action<Message> OnMessage;

        public event Action OnClose;

        public event Action OnOpen;

        public async Task StartSocketAsync(int port)
        {
            _socket = new WebSocket($"ws://127.0.0.1:{port}");
            
            _socket.OnMessage += (sender, args) =>
            {
                _buffer += args.Data;
                Debug.WriteLine($"<-- {args.Data}");
                var split = _buffer.Split(Separator);

                foreach (var message in split.Where(x => !string.IsNullOrEmpty(x)))
                {
                    OnMessage?.Invoke(JsonConvert.DeserializeObject<Message>(message));
                }

                _buffer = split.Last();
            };

            _socket.OnClose += (sender, args) => OnClose?.Invoke();
            _socket.OnOpen += (sender, args) => OnOpen?.Invoke();

            await ConnectAsync().ConfigureAwait(false);
        }

        private async Task ConnectAsync()
        {
            for (var i = 0; i < 60; i++)
            {
                if (_socket.ReadyState != WebSocketState.Open)
                {
                    _socket.Connect();
                    await Task.Delay(1000).ConfigureAwait(false);
                }
                else
                {
                    return;
                }
            }

            throw new ApplicationException("bla-bla");
        }

        public void Send(Message message)
        {
            _socket.SendAsync($"{message.ToJson()}{Separator}", b => {});
        }

        public void Dispose()
        {
            _socket?.Close();
            _socket = null;
        }
    }
}