using System;
using System.Collections.Generic;
using System.Linq;
using BASApi.CSharp.Interfaces;
using BASApi.CSharp.Objects;
using WebSocketSharp;

namespace BASApi.CSharp
{
    // TODO: Should this class be static or singleton?
    public sealed class BasApi : IBasApi
    {
        /// <summary>
        /// </summary>
        private readonly List<IBasBrowser> _browsers = new List<IBasBrowser>();

        /// <summary>
        /// </summary>
        private readonly List<IBasTask> _tasks = new List<IBasTask>();

        private string _buffer;
        private WebSocket _socket;

        /// <inheritdoc />
        public int Send(string type, object data, bool async = false)
        {
            return 0;
        }

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

            _socket.Connect();
        }

        /// <inheritdoc />
        public void ShowBrowser(int browserId)
        {
            var browser = new BasBrowser {BrowserId = browserId};
            Send("show_browser", browser);
        }

        /// <inheritdoc />
        public void HideBrowser(int browserId)
        {
            var browser = new BasBrowser {BrowserId = browserId};
            Send("hide_browser", browser);
        }

        /// <inheritdoc />
        public IReadOnlyList<IBasBrowser> GetBrowsers()
        {
            return _browsers.ToList();
        }

        /// <inheritdoc />
        public IReadOnlyList<IBasTask> GetTasks()
        {
            return _tasks.ToList();
        }

        /// <inheritdoc />
        public void SetGlobalVariable(string name, object value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void GetGlobalVariable(string name)
        {
            throw new NotImplementedException();
        }

        public IBasFunction RunFunction(string functionName, params (string Key, object Value)[] functionParameters)
        {
            throw new NotImplementedException();
        }
    }
}