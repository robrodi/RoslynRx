using System;
using System.Reactive.Linq;

namespace RoslynRx
{
    public class Filter<TEvent> : Thing<TEvent>
    {
        private readonly Func<TEvent, bool> predicate;

        public Filter(string query)
        {
            Query = query;
            var session = CreateSession();
            try
            {
                Log.Info("Compiling Query: {0}", query);
                using (new LogTimer("Compliation", Log))
                {
                    predicate = session.Execute<Func<TEvent, bool>>(query);
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
            return input => input.Where(predicate);
        }
    }
}