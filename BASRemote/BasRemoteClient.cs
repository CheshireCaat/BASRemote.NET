using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BASRemote
{
    using Common;
    using Helpers;
    using Objects;
    using Promises;
    using Services;

    /// <inheritdoc cref="IBasRemoteClient" />
    public sealed class BasRemoteClient : IBasRemoteClient
    {
        private readonly ConcurrentDictionary<int, object> _requests = new ConcurrentDictionary<int, object>();

        private readonly Dictionary<int, IBasBrowser> _browsers = new Dictionary<int, IBasBrowser>();

        private readonly Dictionary<int, IBasTask> _tasks = new Dictionary<int, IBasTask>();

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0, 1);

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
                switch (message.Type)
                {
                    case "browser_remove":
                        RemoveBrowser(message.Data);
                        break;
                    case "browser_add":
                        AddBrowser(message.Data);
                        break;
                    case "thread_start":
                        _semaphore.Release();
                        break;
                    case "thread_end":
                        //
                        break;
                    case "initialize":
                        Send("accept_resources", new Params
                        {
                            {"-bas-empty-script-", true}
                        });
                        break;
                    default:
                    {
                        if (message.Async && message.Id != 0)
                        {
                            if (_requests.TryRemove(message.Id, out var function))
                            {
                                if (function.GetType().IsGenericType)
                                {
                                    ((Action<dynamic>) function)(message.Data);
                                }
                                else
                                {
                                    ((Action) function)();
                                }
                            }
                        }
                        else
                        {
                            //callback
                        }

                        break;
                    }
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


        private void AddBrowser(IBasBrowser basBrowser)
        {
            var browserId = basBrowser.BrowserId.Value;
            var taskId = basBrowser.TaskId.Value;

            if (_browsers.TryGetValue(browserId, out var browser))
                browser.TaskId = taskId;
            else
                _browsers[browserId] = new BasBrowser
                {
                    BrowserId = browserId,
                    TaskId = taskId
                };

            if (_tasks.TryGetValue(taskId, out var task)) task.TaskId = taskId;
        }

        private void RemoveBrowser(IBasBrowser basBrowser)
        {
            var browserId = basBrowser.BrowserId.Value;
            int? taskId = null;

            if (_browsers.TryGetValue(browserId, out var browser)) taskId = browser.TaskId;

            _browsers.Remove(browserId);

            if (taskId != null && _tasks.TryGetValue((int) taskId, out var task)) task.BrowserId = null;
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
        public IPromise<string> OpenFileDialog(FileDialogOptions options)
        {
            return SendAsync<string>("open_file_dialog", options.ToParams());
        }

        /// <inheritdoc />
        public IPromise<string> SaveFileDialog(FileDialogOptions options)
        {
            return SendAsync<string>("save_file_dialog", options.ToParams());
        }

        public IPromise SetGlobalVariable(string name, dynamic value)
        {
            return SendAsync("set_global_variable",
                new Params
                {
                    {"value", value},
                    {"name", name}
                });
        }

        public IPromise<dynamic> GetGlobalVariable(string name)
        {
            return SendAsync<dynamic>("get_global_variable",
                new Params
                {
                    {"name", name}
                });
        }

        /// <inheritdoc />
        public IPromise<string> GetResourcesReport()
        {
            return SendAsync<string>("resources_report");
        }

        /// <inheritdoc />
        public IPromise<string> GetScriptReport()
        {
            return SendAsync<string>("script_report");
        }

        /// <inheritdoc />
        public IPromise<string> DownloadResult(int number)
        {
            return SendAsync<string>("download_result",
                new Params
                {
                    {"number", number}
                });
        }

        /// <inheritdoc />
        public IPromise<string> DownloadLog()
        {
            return SendAsync<string>("download_log");
        }

        /// <inheritdoc />
        public void ShowBrowser(int browserId)
        {
            Send("show_browser",
                new Params
                {
                    {"browser_id", browserId}
                });
        }

        /// <inheritdoc />
        public void HideBrowser(int browserId)
        {
            Send("hide_browser",
                new Params
                {
                    {"browser_id", browserId}
                });
        }

        /// <inheritdoc />
        public IBasThread CreateThread()
        {
            return new BasThread(this);
        }

        /// <inheritdoc />
        public void InstallScheduler()
        {
            Send("install_scheduler");
        }

        /// <inheritdoc />
        public void ShowScheduler()
        {
            Send("show_scheduler");
        }

        /// <inheritdoc />
        public async Task<T> SendAndWaitAsync<T>(string type, Params data = null)
        {
            var tcs = new TaskCompletionSource<T>();

            SendAsync<T>(type, data)
                .Then(result => tcs.TrySetResult(result))
                .Catch(exception => tcs.TrySetException(exception));

            return await tcs.Task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task SendAndWaitAsync(string type, Params data = null)
        {
            var tcs = new TaskCompletionSource<object>();

            SendAsync(type, data)
                .Then(() => tcs.TrySetResult(null))
                .Catch(exception => tcs.TrySetException(exception));

            await tcs.Task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public IPromise<T> SendAsync<T>(string type, Params data = null)
        {
            var message = new Message(data ?? Params.Empty, type, true);
            return new Promise<T>((resolve, reject) =>
            {
                _socket.SendAsync(message);
                _requests[message.Id] = resolve;
            });
        }

        /// <inheritdoc />
        public IPromise SendAsync(string type, Params data = null)
        {
            var message = new Message(data ?? Params.Empty, type, true);
            return new Promise((resolve, reject) =>
            {
                _socket.SendAsync(message);
                _requests[message.Id] = resolve;
            });
        }

        /// <inheritdoc />
        public int Send(string type, Params data, bool async = false)
        {
            var message = new Message(data, type, async);
            _socket.SendAsync(message);
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