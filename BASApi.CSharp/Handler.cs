using System.Collections.Concurrent;
using System.Reactive.Subjects;
using BASRemote.Objects;

namespace BASRemote
{
    internal sealed class Handler
    {
        private readonly ConcurrentDictionary<int, object> _requests;

        public Handler()
        {
            _requests = new ConcurrentDictionary<int, object>();
        }

        public void Call<TResult>(int eventId, TResult data)
        {
            if (_requests.TryGetValue(eventId, out var subject)) ((Subject<TResult>) subject).OnNext(data);
        }

        public IPromise<TResult> Add<TResult>(int eventId)
        {
            var subject = new Subject<TResult>();
            _requests.TryAdd(eventId, subject);
            return new Promise<TResult>(subject);
        }

        public bool Contains(int eventId)
        {
            return _requests.ContainsKey(eventId);
        }
    }
}