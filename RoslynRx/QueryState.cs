using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Roslyn.Scripting.CSharp;

namespace RoslynRx
{
    public class QueryState<T>
    {
        public List<Func<T, bool>> Filters { get; private set; }

        public QueryState()
        {
            Filters = new List<Func<T, bool>>();
        }
    }

    /// <summary>
    /// To be used as the base chaining operation on the <see cref="IObservable{T}"/>
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class Thing<TEvent>
    {
        public string Query { get; protected set; }
        public abstract Func<IObservable<TEvent>, IObservable<TEvent>> GetAction();

    }

    public class Filter<TEvent> : Thing<TEvent>
    {
        public Filter(string query)
        {
            Query = query;
            var session = new ScriptEngine().CreateSession();
            session.AddReference(typeof(TEvent).Assembly);
            try
            {
            }
            catch { }

        }

        public override Func<IObservable<TEvent>, IObservable<TEvent>> GetAction()
        {
            

            return i => i.Where(@event => @event != null);
        }
    }
}