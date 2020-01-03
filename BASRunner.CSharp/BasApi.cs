using System;
using System.Linq;
using BASRunner.CSharp.Interfaces;
using WebSocketSharp;

namespace BASRunner.CSharp
{
    // TODO: Should this class be static or singleton?
    public sealed class BasApi : IBasApi
    {
        private string _buffer;
        private WebSocket _socket;

        /// <inheritdoc />
        public void Init(int port)
        {
            _socket = new WebSocket($"ws://127.0.0.1:{port}");

            _socket.OnClose += (sender, args) => { };

            _socket.OnMessage += (sender, args) =>
            {
                _buffer += args.Data;
                var split = _buffer.Split(
                    new[] {"---Message--End---"},
                    StringSplitOptions.None
                );

                foreach (var message in split)
                {
                }

                _buffer = split.Last();
            };
        }

        /// <inheritdoc />
        public void ShowBrowser(int browserId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void HideBrowser(int browserId)
        {
            throw new NotImplementedException();
        }

        public object GetTasks()
        {
            throw new NotImplementedException();
        }

        public IBasFunction RunFunction(string functionName, params (string Key, object Value)[] functionParameters)
        {
            throw new NotImplementedException();
        }
    }
}