using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using NLog;

namespace RoslynRx
{
    /// <summary>
    /// Demultiplexes a collection of events into a number of <see cref="IObservable{T}"/>s.
    /// </summary>
    /// <example>
    ///     var knownTypes = new[] { 1, 2 };
    ///     var dm = new Demuxer&lt;int, Event&lt;long&gt;&gt;(knownTypes, "@event =&gt; @event.Type");
    ///     var testInterval = new TestInterval(100, 5);
    ///     testInterval.Interval.Subscribe(dm);
    ///     foreach (var type in knownTypes{
    ///     {
    ///         dm.Streams[type].Count().Subscribe(...);
    ///     }
    /// </example>
    /// <typeparam name="TKey">The type of the key to use, each of which have an entry in <see cref="Streams"/>.</typeparam>
    /// <typeparam name="TEvent">The type of event to pass through.</typeparam>
    public class Demuxer<TKey, TEvent> : IObserver<TEvent>
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        private readonly Func<TEvent, TKey> keySelector;
        private readonly IDictionary<TKey, Subject<TEvent>> subjects;
        public IDictionary<TKey, IObservable<TEvent>> Streams { get; private set; }

        public IEnumerable<TKey> KnownKeys { get; private set; }

        public Demuxer(IEnumerable<TKey> keys, string query)
        {
            TKey[] array = keys.ToArray();
            subjects = array.ToDictionary(key => key, key => new Subject<TEvent>());
            Streams = subjects.ToDictionary(entry => entry.Key, entry => (IObservable<TEvent>) entry.Value);
            KnownKeys = array;

            var session = Predicate<TEvent>.CreateSession();

            try
            {
                Log.Info("Compiling Query: {0}", query);
                using (new LogTimer("Compilation", Log))
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
            Log.LogException(LogLevel.Fatal, "Exception encountered", error);
        }

        public void OnCompleted()
        {
            foreach (var subject in subjects)
                subject.Value.OnCompleted();
        }
    }
}