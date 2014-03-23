using System;
using System.Reactive.Linq;

namespace RoslynRx
{
    /// <summary>
    /// Basically an aggregator generator.
    /// </summary>
    /// <example>
        /// var summer = new Aggregate&lt;Event&lt;long&gt;&gt;("(sum, @event) =&gt; new RoslynRx.Tests.Event&lt;long&gt;(0, sum.Data + @event.Data)");
    /// </example>
    /// <typeparam name="TEvent">The type of event</typeparam>
    [Obsolete("Doesn't work great on hot observables, as it waits until the end.  Use scan instaed")]
    public class Aggregate<TEvent> : Predicate<TEvent>
    {
        private Func<IObservable<TEvent>, IObservable<TEvent>> result;

        public Aggregate(string query)
        {
            var session = CreateSession();

            try
            {
                Log.Info("Compiling Query: {0}", query);
                using (new LogTimer("Compilation", Log))
                {
                    result = input => input.Aggregate(session.Execute<Func<TEvent, TEvent, TEvent>>(query));
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(string.Format("Failed to compile query {0}", query), ex);
                throw;
            }
        }

        public override Func<IObservable<TEvent>, IObservable<TEvent>> GetAction()
        {
            return result;
        }
    }
}