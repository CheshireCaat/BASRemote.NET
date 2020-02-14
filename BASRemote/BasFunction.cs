using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.Extensions;
using BASRemote.Helpers;
using BASRemote.Objects;

namespace BASRemote
{
    /// <inheritdoc cref="IBasFunction" />
    internal sealed class BasFunction : IBasFunction, IFunctionRunner<IBasFunction>
    {
        private TaskCompletionSource<dynamic> _completion;

        /// <summary>
        ///     Create an instance of <see cref="BasFunction" /> class.
        /// </summary>
        /// <param name="client">
        ///     Remote client object.
        /// </param>
        public BasFunction(IBasRemoteClient client)
        {
            Client = client;
        }

        /// <summary>
        ///     Remote client object.
        /// </summary>
        private IBasRemoteClient Client { get; }

        /// <inheritdoc />
        public int Id { get; private set; }

        /// <inheritdoc />
        public IBasFunction RunFunction(string functionName, Params functionParams)
        {
            _completion = new TaskCompletionSource<dynamic>();

            RunFunction(functionName, functionParams,
                result => _completion.TrySetResult(result),
                exception => _completion.TrySetException(exception));

            return this;
        }

        /// <inheritdoc />
        public IBasFunction RunFunction(string functionName)
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
                if (!task.IsFaulted)
                {
                    completion.TrySetResult(((object) task.Result).Convert<TResult>());
                }
                else
                {
                    completion.TrySetException(task.Exception.InnerExceptions);
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
            Client.Send("stop_thread", new Params {{"thread_id", Id}});
        }

        private void RunFunction(
            string functionName,
            Params functionParams,
            Action<dynamic> onResult,
            Action<Exception> onError)
        {
            Id = Rand.NextInt(1, 1000000);
            Client.Send("start_thread", new Params {{"thread_id", Id}});

            Client.SendAsync<string>("run_task",
                new Params
                {
                    ["params"] = functionParams.ToJson(),
                    ["function_name"] = functionName,
                    ["thread_id"] = Id
                }, result =>
               {
                    var response = result.FromJson<Response>();

                    if (!response.Success)
                    {
                        onError(new FunctionException(response.Message));
                    }
                    else
                    {
                        onResult(response.Result);
                    }

                    Client.Send("stop_thread", new Params {{"thread_id", Id}});
                });
        }
    }
}