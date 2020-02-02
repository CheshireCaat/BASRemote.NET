using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BASRemote.Objects
{
    internal sealed class Promise<T> : IPromise<T>
    {
        private readonly Subject<T> _subject;

        public Promise(Subject<T> subject)
        {
            _subject = subject;
        }

        public IPromise<T> Then(Action<T> callback)
        {
            _subject.Where(x => !(x is Exception)).Take(1).Subscribe(callback);
            return this;
        }

        public IPromise<T> Catch(Action<Exception> callback)
        {
            _subject.Where(x => x is Exception).Take(1).Subscribe(obj =>
            {
                callback.Invoke(obj as Exception);
            });
            return this;
        }
    }
}