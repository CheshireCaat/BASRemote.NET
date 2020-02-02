using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BASRemote.Helpers;
using BASRemote.Objects;

namespace BASRemote.Common
{
    /// <inheritdoc cref="IBasThread" />
    internal sealed class BasThread : IBasThread
    {
        /// <summary>
        ///     Remote client object.
        /// </summary>
        private readonly IBasRemoteClient _client;

        /// <summary>
        ///     Create an instance of <see cref="BasThread" /> class.
        /// </summary>
        /// <param name="client">
        ///     Remote client object.
        /// </param>
        internal BasThread(IBasRemoteClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <inheritdoc />
        public int Id { get; private set; }

        /// <inheritdoc />
        public Task<object> RunFunctionAsync(string functionName, Params functionParams)
        {
            var tcs = new TaskCompletionSource<object>();

            RunFunction(functionName, functionParams)
                .Then(result => tcs.TrySetResult(result))
                .Catch(exception => tcs.TrySetException(exception));

            return tcs.Task;
        }

        /// <inheritdoc />
        public IPromise<object> RunFunction(string functionName, Params functionParams)
        {
            if (Id == 0 && IsRunning)
                throw new ApplicationException("Other task is executing, can't start new task one");

            if (Id == 0)
            {
                Id = RandomHelper.GenerateThreadId();
                _client.Send("start_thread", new Params
                {
                    {"thread_id", Id}
                });
            }

            var subject = new Subject<object>();

            _client.AddTask(Id, "thread", this, functionName, functionParams);
            _client.SendAsync<FunctionResult>("run_task", new Params
            {
                {"params", functionParams.AsJson()},
                {"function_name", functionName},
                {"thread_id", Id}
            }).Then(data =>
            {
                _client.RemoveTask(Id);
                IsRunning = false;

                if (data.Success)
                    subject.OnNext(data.Result);
                else
                    subject.OnNext(new ApplicationException(data.Message));
            });

            return new Promise<object>(subject);
        }

        public void Stop()
        {
            if (Id != 0)
            {
                _client.RemoveTask(Id);
                _client.Send("stop_thread", new Params
                {
                    {"thread_id", Id}
                });
            }

            IsRunning = false;
            Id = 0;
        }
    }
}