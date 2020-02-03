using System;

namespace BASRemote.Promises
{
    public interface IPromise
    {
        IPromise Then(Action onResolved);

        IPromise Catch(Action<Exception> onRejected);
    }
}