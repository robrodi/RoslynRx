using System;
using System.Reactive.Linq;

namespace RoslynRx
{
    /// <summary>
    /// Basically an aggregator generator.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public class Scan<TEvent> : Predicate<TEvent>
    {
        private Func<IObservable<TEvent>, IObservable<TEvent>> result;

        public Scan(string query)
        {
            var session = CreateSession();

            try
            {
                Log.Info("Compiling Query: {0}", query);
                using (new LogTimer("Compliation", Log))
                {
                    result = input => input.Scan(session.Execute<Func<TEvent, TEvent, TEvent>>(query));
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