using System;
using System.Diagnostics;
using System.Linq;
using BASApi.CSharp.Objects;
using BASApi.CSharp.Utility;
using Newtonsoft.Json;
using WebSocketSharp;

namespace BASApi.CSharp.Services
{
    /// <summary>
    /// </summary>
    internal sealed class SocketService : BaseService
    {
        private static readonly Random Rand = new Random();
        private string _buffer;

        private WebSocket _socket;

        /// <summary>
        /// </summary>
        /// <param name="options"></param>
        public SocketService(BasRemoteOptions options) : base(options)
        {
        }

        private void A()
        {
            _socket = new WebSocket($"ws://127.0.0.1:{Rand.Next(0, 1)}");

            _socket.OnMessage += (sender, args) =>
            {
                Debug.WriteLine(args.Data);
                _buffer += args.Data;

                var split = _buffer.Split("---Message--End---");

                foreach (var message in split)
                {
                    var msg = JsonConvert.DeserializeObject<BasMessage>(message);
                }

                _buffer = split.Last();
            };

            _socket.Connect();
        }
    }
}