using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BASRemote.Common;
using BASRemote.Helpers;
using BASRemote.Objects;
using BASRemote.Services;
using Newtonsoft.Json;

namespace BASRemote
{
    /// <inheritdoc cref="IBasRemoteClient" />
    public sealed class BasRemoteClient : IBasRemoteClient
    {
        private readonly Dictionary<int, IBasBrowser> _browsers = new Dictionary<int, IBasBrowser>();

        private readonly Dictionary<int, IBasTask> _tasks = new Dictionary<int, IBasTask>();

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0, 1);

        private readonly Handler _handler = new Handler();

        private EngineService _engine;

        private SocketService _socket;

        /// <summary>
        ///     Create an instance of <see cref="BasRemoteClient" /> class.
        /// </summary>
        /// <param name="options">
        ///     Remote control options.
        /// </param>
        public BasRemoteClient(BasRemoteOptions options)
        {
            Browsers = new ReadOnlyDictionary<int, IBasBrowser>(_browsers);
            Tasks = new ReadOnlyDictionary<int, IBasTask>(_tasks);

            _engine = new EngineService(options);
            _socket = new SocketService(options);

            _socket.OnMessage += message =>
            {
                if (message.Type == "browser_add")
                {
                    AddBrowser((int)message.Data.browser_id, (int)message.Data.task_id);
                }
                else if (message.Type == "browser_remove")
                {
                    RemoveBrowser(message.Data.browser_id);
                }
                else if (message.Type == "thread_start")
                {
                    _semaphore.Release();
                }
                else if (message.Type == "thread_end")
                {
                    //
                }
                else if (message.Type == "initialize")
                {
                    Send("accept_resources", new Params
                    {
                        {"-bas-empty-script-", true}
                    });
                }
                else if (message.Async && message.Id != 0)
                {
                    _handler.Call(message.Id, message.Data);
                }
            };

            _socket.OnOpen += () =>
            {
                Send("remote_control_data", new Params
                {
                    {"script", _socket.Options.ScriptName},
                    {"password", _socket.Options.Password},
                    {"login", _socket.Options.Login}
                });
            };
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<int, IBasBrowser> Browsers { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<int, IBasTask> Tasks { get; }

        internal bool CanSchedule { get; private set; }

        /// <inheritdoc />
        public void AddBrowser(int browserId, int taskId)
        {
            if (_browsers.TryGetValue(browserId, out var browser))
            {
                browser.TaskId = taskId;
            }
            else
            {
                _browsers[browserId] = new BasBrowser
                {
                    BrowserId = browserId,
                    TaskId = taskId
                };
            }

            if (_tasks.TryGetValue(taskId, out var task)) task.TaskId = taskId;
        }

        /// <inheritdoc />
        public void RemoveBrowser(int browserId)
        {
            int? taskId = null;

            if (_browsers.TryGetValue(browserId, out var browser))
            {
                taskId = browser.TaskId;
            }

            _browsers.Remove(browserId);

            if (taskId != null && _tasks.TryGetValue((int) taskId, out var task))
            {
                task.BrowserId = null;
            }
        }

        public void AddTask(int id, string thread, IBasThread basThread, string functionName, Params functionParams)
        {
            var browserId = _browsers
                .Where(x => x.Value.TaskId == id)
                .Select(x => x.Value.BrowserId)
                .FirstOrDefault();

            if (browserId.HasValue) _tasks.Add(browserId.Value, new BasTask());
        }

        public void RemoveTask(int id)
        {
            _tasks.Remove(id);
        }

        public IBasThread CreateThread()
        {
            return new BasThread(this);
        }

        /// <inheritdoc />
        public async Task StartAsync()
        {
            await _engine.InitializeAsync().ConfigureAwait(false);

            var port = RandomHelper.GeneratePort();

            await _engine.StartEngineAsync(port)
                .ConfigureAwait(false);
            await _socket.StartSocketAsync(port)
                .ConfigureAwait(false);

            await _semaphore.WaitAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task StopAsync()
        {
            await Task.Yield();
        }

        /// <inheritdoc />
        public Task<T> SendAndWaitAsync<T>(string type, Params data)
        {
            var tcs = new TaskCompletionSource<T>();

            SendAsync<T>(type, data)
                .Then(result => tcs.TrySetResult(result))
                .Catch(exception => tcs.TrySetException(exception));

            return tcs.Task;
        }

        /// <inheritdoc />
        public Task<T> SendAndWaitAsync<T>(string type)
        {
            return SendAndWaitAsync<T>(type, Params.Empty);
        }

        /// <inheritdoc />
        public IPromise<T> SendAsync<T>(string type, Params data)
        {
            var message = new Message(data, type, true);
            var promise = _handler.Add<T>(message.Id);
            _socket.Send(message);
            return promise;
        }

        /// <inheritdoc />
        public IPromise<T> SendAsync<T>(string type)
        {
            return SendAsync<T>(type, Params.Empty);
        }

        /// <inheritdoc />
        public int Send(string type, Params data, bool async = false)
        {
            var message = new Message(data, type, async);
            _socket.Send(message);
            return message.Id;
        }

        /// <inheritdoc />
        public int Send(string type, bool async = false)
        {
            return Send(type, Params.Empty, async);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _engine?.Dispose();
            _socket?.Dispose();
            _engine = null;
            _socket = null;
        }
    }
}