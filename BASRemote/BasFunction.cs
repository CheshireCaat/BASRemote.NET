using System;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.Extensions;
using BASRemote.Helpers;
using BASRemote.Interfaces;
using BASRemote.Objects;

namespace BASRemote
{
    /// <inheritdoc cref="IBasFunction" />
    internal sealed class BasFunction : IBasFunction, IClientContainer, IFunctionRunner<IBasFunction>
    {
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

        /// <inheritdoc />
        public IBasRemoteClient Client { get; }

        /// <inheritdoc />
        public int Id { get; private set; }

        /// <inheritdoc />
        public IBasFunction RunFunctionSync(string functionName, Params functionParams, Action<dynamic> onResult,
            Action<Exception> onError)
        {
            Id = Rand.NextInt(1, 1000000);
            Client.Send("start_thread", new Params {{"thread_id", Id}});

            Client.SendAsync<string>("run_task",
                new Params
                {
                    {"params", functionParams.ToJson()},
                    {"function_name", functionName},
                    {"thread_id", Id}
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

            return this;
        }

        /// <inheritdoc />
        public async Task<TResult> RunFunction<TResult>(string functionName, Params functionParams)
        {
            var tcs = new TaskCompletionSource<TResult>();

            RunFunctionSync(functionName, functionParams,
                result => tcs.TrySetResult(((object) result).Convert<TResult>()),
                exception => tcs.TrySetException(exception));

            return await tcs.Task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<dynamic> RunFunction(string functionName, Params functionParams)
        {
            var tcs = new TaskCompletionSource<dynamic>();

            RunFunctionSync(functionName, functionParams,
                result => tcs.TrySetResult(result),
                exception => tcs.TrySetException(exception));

            return await tcs.Task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Stop()
        {
            Client.Send("stop_thread", new Params {{"thread_id", Id}});
        }
    }
}