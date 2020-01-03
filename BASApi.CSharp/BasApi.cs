using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BASApi.CSharp.Interfaces;
using BASApi.CSharp.Objects;
using Newtonsoft.Json;
using WebSocketSharp;

namespace BASApi.CSharp
{
    // TODO: Should this class be static or singleton?
    public sealed class BasApi : IBasApi
    {
        private static readonly Random _sendRandom = new Random();

        /// <summary>
        /// </summary>
        private readonly List<IBasBrowser> _browsers = new List<IBasBrowser>();

        /// <summary>
        /// </summary>
        private readonly List<IBasTask> _tasks = new List<IBasTask>();

        private string _buffer = "";
        private WebSocket _socket;

        /// <inheritdoc />
        public int Send(string type, object data, bool async = false)
        {
            var id = _sendRandom.Next(100000, 999999);

            dynamic d = new {type, data, id};

            if (async)
                d.async = true;

            var msg = JsonConvert.SerializeObject(d) + "---Message--End---";
            Debug.WriteLine($"[SEND] - {msg}");
            _socket.Send(msg);
            return id;
        }

        // TODO: Get correct port without user interface?
        /// <inheritdoc />
        public void Init(int port)
        {
            _socket = new WebSocket($"ws://127.0.0.1:{port}");

            _socket.OnClose += (sender, args) => { };

            _socket.OnMessage += (sender, args) =>
            {
                Debug.WriteLine(args.Data);
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

        /// <inheritdoc />
        public void Stop(bool isInstant)
        {
            object obj = new {is_instant = isInstant};
            Send("stop", obj);
        }

        public IBasFunction RunFunction(string functionName, params (string Key, object Value)[] functionParameters)
        {
            throw new NotImplementedException();
        }
    }
}