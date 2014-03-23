using System;
using System.Reactive.Linq;

namespace RoslynRx
{
    /// <summary>
    /// More or less a where clause generator.
    /// </summary>
    /// <typeparam name="TEvent">The type of the thing.</typeparam>
    public class Filter<TEvent> : Predicate<TEvent>
    {
        private readonly Func<IObservable<TEvent>, IObservable<TEvent>> predicate;

        public Filter(string query)
        {
            Query = query;
            var session = CreateSession();
            try
            {
                Log.Info("Compiling Query: {0}", query);
                using (new LogTimer("Compliation", Log))
                {
                    predicate = input => input.Where(session.Execute<Func<TEvent, bool>>(query));
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
            return predicate;
        }
    }
}