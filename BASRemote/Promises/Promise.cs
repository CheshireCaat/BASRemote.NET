using System;

namespace BASRemote.Promises
{
    internal sealed class Promise : IPromise
    {
        private readonly RSG.IPromise _promise;

        private Promise(RSG.IPromise promise)
        {
            _promise = promise;
        }

        public Promise(Action<Action, Action<Exception>> resolver)
        {
            _promise = new RSG.Promise(resolver);
        }

        public IPromise Then(Action onResolved)
        {
            _promise.Then(onResolved);
            return this;
        }

        public IPromise Catch(Action<Exception> onRejected)
        {
            _promise.Catch(onRejected);
            return this;
        }

        public static IPromise Resolve()
        {
            return new Promise(RSG.Promise.Resolved());
        }

        public static IPromise Reject(Exception ex)
        {
            return new Promise(RSG.Promise.Rejected(ex));
        }
    }
}