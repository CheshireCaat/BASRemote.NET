using System;

namespace BASRemote.Objects
{
    public interface IPromise<out T>
    {
        IPromise<T> Then(Action<T> callback);

        IPromise<T> Catch(Action<Exception> callback);
    }
}