using System;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.Helpers;
using BASRemote.Objects;
using BASRemote.Promises;
using Newtonsoft.Json;

namespace BASRemote.Common
{
    /// <inheritdoc cref="IBasThread" />
    internal sealed class BasThread : IBasThread
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

        /// <summary>
        ///     Remote client object.
        /// </summary>
        private IBasRemoteClient Client { get; }

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <inheritdoc />
        public int Id { get; private set; }

        /// <inheritdoc />
        public async Task<dynamic> RunFunctionAsync(string functionName, Params functionParams)
        {
            var tcs = new TaskCompletionSource<dynamic>();

            RunFunction(functionName, functionParams)
                .Then(result => tcs.TrySetResult(result))
                .Catch(exception => tcs.TrySetException(exception));

            return await tcs.Task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public IPromise<dynamic> RunFunction(string functionName, Params functionParams)
        {
            if (Id != 0 && IsRunning) return Promise<dynamic>.Reject(new AlreadyRunningException());

            if (Id == 0)
            {
                Id = RandomHelper.GenerateThreadId();
                Client.Send("start_thread",
                    new Params
                    {
                        {"thread_id", Id}
                    });
            }

            return new Promise<dynamic>((resolve, reject) =>
            {
                Client.AddTask(Id, "thread", this, functionName, functionParams);
                Client.SendAsync<dynamic>("run_task",
                    new Params
                    {
                        {"params", functionParams.ToJson()},
                        {"function_name", functionName},
                        {"thread_id", Id}
                    }).Then(data =>
                    {
                        var result = JsonConvert.DeserializeObject<FunctionResult>(data);
                        Client.RemoveTask(Id);
                        IsRunning = false;

                        if (result.Success)
                            resolve(result.Result);
                        else
                            reject(new ApplicationException(result.Message));
                    });
            });
        }

        /// <inheritdoc />
        public void Stop()
        {
            if (Id != 0)
            {
                Client.RemoveTask(Id);
                Client.Send("stop_thread",
                    new Params
                    {
                        {"thread_id", Id}
                    });
            }

            IsRunning = false;
            Id = 0;
        }
    }
}