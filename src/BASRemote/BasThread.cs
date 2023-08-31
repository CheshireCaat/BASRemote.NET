using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.Extensions;
using BASRemote.Helpers;
using BASRemote.Objects;

namespace BASRemote
{
    /// <inheritdoc cref="IBasThread" />
    internal sealed class BasThread : IBasThread
    {
        private TaskCompletionSource<dynamic> _completion;

        /// <summary>
        ///     Create an instance of <see cref="BasThread" /> class.
        /// </summary>
        /// <param name="client">
        ///     Remote client object.
        /// </param>
        public BasThread(IBasRemoteClient client)
        {
            Client = client;
        }

        /// <summary>
        ///     Remote client object.
        /// </summary>
        private IBasRemoteClient Client { get; }

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <inheritdoc />
        public int Id { get; private set; }

        /// <inheritdoc />
        public IBasThread RunFunction(string functionName, Params functionParams)
        {
            _completion = new TaskCompletionSource<dynamic>();

            RunFunction(functionName, functionParams,
                result => _completion.TrySetResult(result),
                exception => _completion.TrySetException(exception));

            return this;
        }

        /// <inheritdoc />
        public IBasThread RunFunction(string functionName)
        {
            return RunFunction(functionName, Params.Empty);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public Task<TResult> GetTask<TResult>()
        {
            var completion = new TaskCompletionSource<TResult>();

            _completion.Task.ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    completion.TrySetException(task.Exception.InnerExceptions);
                }
                else
                {
                    completion.TrySetResult(((object)task.Result).Convert<TResult>());
                }
            });

            return completion.Task;
        }

        /// <inheritdoc />
        public Task<dynamic> GetTask()
        {
            return _completion.Task;
        }

        /// <inheritdoc />
        public void Stop()
        {
            if (Id != 0)
            {
                Client.Send("stop_thread", new Params {{"thread_id", Id}});
            }

            IsRunning = false;
            Id = 0;
        }

        private void RunFunction(
            string functionName,
            Params functionParams,
            Action<dynamic> onResult,
            Action<Exception> onError)
        {
            if (Id != 0 && IsRunning)
            {
                onError(new AlreadyRunningException());
            }

            if (Id == 0)
            {
                Id = Rand.NextInt(1, 1000000);
                Client.Send("start_thread", new Params {{"thread_id", Id}});
            }

            Client.SendAsync<string>("run_task",
                new Params
                {
                    ["params"] = functionParams.ToJson(),
                    ["function_name"] = functionName,
                    ["thread_id"] = Id
                }, result =>
                {
                    var response = result.FromJson<Response>();
                    IsRunning = false;

                    if (response.Success)
                    {
                        onResult(response.Result);
                    }
                    else
                    {
                        onError(new FunctionException(response.Message));
                    }
                });

            IsRunning = true;
        }
    }
}