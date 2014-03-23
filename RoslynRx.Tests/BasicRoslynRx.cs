using System;
using System.Reactive.Linq;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace RoslynRx.Tests
{
    [TestClass]
    public class BasicRoslynRx
    {
        public Func<EventBase, bool> filter;

        [TestMethod]
        public void MostBasic_Count()
        {
            // Where type = 1

            var scheduler = new TestScheduler();
            const int expectedCount = 60;
            const int numberOfTypes = 2;

            var interval = Observable.Interval(TimeSpan.FromSeconds(1), scheduler).Take(expectedCount).Select(i => new Event<long>((int)i % numberOfTypes, i));

            var query = new QueryState<EventBase>();

            var session = ConfigureSession(query);
            session.Execute("Filters.Add(@event => @event.Type == 1);");

            int count = 0;
            interval.WhereEach(query.Filters).Count().Subscribe(i => count = i);
            scheduler.Start();
            count.Should().Be(expectedCount / numberOfTypes);
        }

        [TestMethod]
        public void Compiler_SystemType()
        {
            var session = new ScriptEngine().CreateSession();
            session.AddReference(typeof (QueryState<EventBase>).Assembly);
            session.AddReference(typeof(Func<int, bool>).Assembly);
            var actual = session.Execute<Func<int, bool>>("i => i == 1");
            actual.Should().NotBeNull("null compilation");
        }

        [TestMethod]
        public void MostBasic_Count_100x()
        {
            // Where type = 1

            var scheduler = new TestScheduler();
            const int expectedCount = 60;
            const int numberOfTypes = 2;

            var interval = Observable.Interval(TimeSpan.FromSeconds(1), scheduler).Take(expectedCount).Select(i => new Event<long>((int)i % numberOfTypes, i));

            var query = new QueryState<EventBase>();

            var session = ConfigureSession(query);
            session.Execute("Filters.Add(@event => @event.Type == 1);");

            int count = 0;
            interval.WhereEach(query.Filters).Count().Subscribe(i => count = i);
            scheduler.Start();
            count.Should().Be(expectedCount / numberOfTypes);
        }

        private Session ConfigureSession(QueryState<EventBase> query)
        {
            var session = new ScriptEngine().CreateSession(query);
            session.AddReference(GetType().Assembly);
            session.AddReference(typeof (QueryState<EventBase>).Assembly);
            return session;
        }
    }
}
