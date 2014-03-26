using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace RoslynRx.Tests
{
    [TestClass]
    public class MegaloTests
    {
        [TestMethod]
        public void CanAccessExternalStreams()
        {
            var testInterval = new TestInterval();
            var inputStream = testInterval.Interval;
            var megalo = new Megalo<Event<long>> (new KeyValuePair<string, IObservable<Event<long>>>("input", inputStream));
            megalo.Execute("Streams.Add(\"Type1s\", Streams[\"input\"].Where(@event => @event.Type == 1))");
            //megalo.Streams.Add("Type1s", megalo.Streams["input"].Where(@event => @event.Type == 1))
            megalo.Streams.Should().ContainKey("Type1s");
            Assert.Inconclusive();
        }
    }

    public class Megalo<TEvent>
    {
        public Megalo(params KeyValuePair<string, IObservable<TEvent>>[] streams)
        {
            foreach (var stream in streams)
            {
                this.streams.Add(stream.Key, (IObservable<Event<long>>) stream.Value);
            }

            session = new ScriptEngine().CreateSession(this);
            Console.WriteLine("Adding reference of {0}", typeof(TEvent).Name);
            session.AddReference(typeof(TEvent).Assembly);
            session.AddReference(typeof(Observable).Assembly);
            session.AddReference(typeof(IQbservable<>).Assembly);
            session.Execute("using System.Reactive.Linq;");
        }

        public void Execute(string query)
        {
            session.Execute(query);
        }

        private readonly Dictionary<string, IObservable<Event<long>>> streams =
            new Dictionary<string, IObservable<Event<long>>>();

        private Session session;

        public Dictionary<string, IObservable<Event<long>>> Streams { get { return streams; } }
    }
}
;