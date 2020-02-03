using System;

namespace BASRemote.Promises
{
    internal sealed class Promise<T> : IPromise<T>
    {
        private readonly RSG.IPromise<T> _promise;

        private Promise(RSG.IPromise<T> promise)
        {
            _promise = promise;
        }

        public Promise(Action<Action<T>, Action<Exception>> resolver)
        {
            _promise = new RSG.Promise<T>(resolver);
        }

        public IPromise<T> Then(Action<T> onResolved)
        {
            _promise.Then(onResolved);
            return this;
        }

        public IPromise<T> Catch(Action<Exception> onRejected)
        {
            _promise.Catch(onRejected);
            return this;
        }

        public static IPromise<T> Resolve(T promisedValue)
        {
            return new Promise<T>(RSG.Promise<T>.Resolved(promisedValue));
        }

        public static IPromise<T> Reject(Exception ex)
        {
            return new Promise<T>(RSG.Promise<T>.Rejected(ex));
        }
    }
}