using System;

namespace BASRemote.Promises
{
    public interface IPromise<out T>
    {
        IPromise<T> Then(Action<T> callback);

        IPromise<T> Catch(Action<Exception> callback);
    }
}