using System;
using NLog;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace RoslynRx
{
    /// <summary>
    /// To be used as the base chaining operation on the <see cref="IObservable{T}"/>
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class Predicate<TEvent>
    {
        protected static Logger Log = LogManager.GetCurrentClassLogger();

        public string Query { get; protected set; }
        public abstract Func<IObservable<TEvent>, IObservable<TEvent>> GetAction();

        internal static Session CreateSession()
        {
            var session = new ScriptEngine().CreateSession();
            Log.Info("Adding reference of {0}", typeof (TEvent).Name);
            session.AddReference(typeof (TEvent).Assembly);
            session.AddReference(typeof (IObservable<>).Assembly);
            return session;
        }
    }
}