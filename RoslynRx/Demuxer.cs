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
        private readonly IDictionary<TKey, Subject<TEvent>> streams;

        public IObservable<TEvent> this[TKey key]
        {
            get
            {
                return streams[key];
            }
        }

        public IEnumerable<TKey> KnownKeys { get; private set; }

        public Demuxer(IEnumerable<TKey> keys, string query)
        {
            streams = keys.ToDictionary(key => key, key => new Subject<TEvent>());
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
                streams[key].OnNext(value);
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
            foreach (var subject in streams)
                subject.Value.OnCompleted();
        }
    }
}