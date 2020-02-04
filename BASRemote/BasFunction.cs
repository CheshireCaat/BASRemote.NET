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
    internal sealed class BasFunction : IBasFunction, IClientContainer
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
        public async Task<dynamic> RunFunction(string functionName, Params functionParams)
        {
            return await RunFunction<dynamic>(functionName, functionParams)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<T> RunFunction<T>(string functionName, Params functionParams)
        {
            var tcs = new TaskCompletionSource<T>();

            RunFunctionSync(functionName, functionParams,
                result => tcs.TrySetResult((T) result),
                exception => tcs.TrySetException(exception));

            return await tcs.Task.ConfigureAwait(false);
        }

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
        public void Stop()
        {
            Client.Send("stop_thread", new Params {{"thread_id", Id}});
        }
    }
}