using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using NLog;

namespace RoslynRx
{
    public class Demuxer<TKey, TEvent> : IObserver<TEvent>
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        private readonly Func<TEvent, TKey> keySelector;
        private readonly IDictionary<TKey, Subject<TEvent>> subjects;
        public IDictionary<TKey, IObservable<TEvent>> Streams { get; private set; }

        public IEnumerable<TKey> KnownKeys { get; private set; }

        public Demuxer(IEnumerable<TKey> keys, string query)
        {
            subjects = keys.ToDictionary(key => key, key => new Subject<TEvent>());
            Streams = subjects.ToDictionary(entry => entry.Key, entry => (IObservable<TEvent>) entry.Value);
            KnownKeys = keys;

            var session = Predicate<TEvent>.CreateSession();

            try
            {
                Log.Info("Compiling Query: {0}", query);
                using (new LogTimer("Compliation", Log))
                {
                    keySelector = session.Execute<Func<TEvent, TKey>>(query);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(string.Format("Failed to compile query {0}", query), ex);
                throw;
            }
        }

        public void OnNext(TEvent value)
        {
            if (value == null)
            {
                Log.Info("Null Event");
                return;
            }

            var key = keySelector(value);
            if (KnownKeys.Contains(key))
            {
                subjects[key].OnNext(value);
            }
            else
                Log.Trace("Uknown Key: {0}", key);
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            foreach (var subject in subjects)
                subject.Value.OnCompleted();
        }
    }
}