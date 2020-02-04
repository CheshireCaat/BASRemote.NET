using System;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.Extensions;
using BASRemote.Helpers;
using BASRemote.Interfaces;
using BASRemote.Objects;

namespace BASRemote
{
    /// <inheritdoc cref="IBasThread" />
    internal sealed class BasThread : IBasThread, IClientContainer
    {
        /// <summary>
        ///     Create an instance of <see cref="BasThread" /> class.
        /// </summary>
        /// <param name="client">
        ///     Remote client object.
        /// </param>
        internal BasThread(IBasRemoteClient client)
        {
            Client = client;
        }

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <inheritdoc />
        public int Id { get; private set; }

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
        public async Task<T> RunFunction<T>(string functionName, Params functionParams)
        {
            var tcs = new TaskCompletionSource<T>();

            RunFunctionSync(functionName, functionParams,
                result => tcs.TrySetResult((T) result),
                exception => tcs.TrySetException(exception));

            return await tcs.Task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public IBasThread RunFunctionSync(
            string name,
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
                    {"params", functionParams.ToJson()},
                    {"function_name", name},
                    {"thread_id", Id}
                }, result =>
                {
                    var response = result.FromJson<Response>();
                    IsRunning = false;

                    if (!response.Success)
                    {
                        onError(new FunctionException(response.Message));
                    }
                    else
                    {
                        onResult(response.Result);
                    }
                });

            return this;
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

        /// <inheritdoc />
        public IBasRemoteClient Client { get; }
    }
}