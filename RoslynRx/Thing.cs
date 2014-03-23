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
    public abstract class Thing<TEvent>
    {
        protected static Logger Log = LogManager.GetCurrentClassLogger();

        public string Query { get; protected set; }
        public abstract Func<IObservable<TEvent>, IObservable<TEvent>> GetAction();

        protected static Session CreateSession()
        {
            var session = new ScriptEngine().CreateSession();
            session.AddReference(typeof (TEvent).Assembly);
            session.AddReference(typeof (IObservable<>).Assembly);
            return session;
        }
    }
}