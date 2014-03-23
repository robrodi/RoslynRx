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

            var testInterval = new TestInterval();

            var query = new QueryState<EventBase>();

            var session = ConfigureSession(query);
            session.Execute("Filters.Add(@event => @event.Type == 1);");

            int count = 0;
            testInterval.Interval.WhereEach(query.Filters).Count().Subscribe(i => count = i);
            testInterval.Start();
            count.Should().Be(testInterval.ExpectedCount / testInterval.NumberOfTypes);
        }

        [TestMethod]
        public void Compiler_SystemType()
        {
            var session = new ScriptEngine().CreateSession();
            session.AddReference(typeof (QueryState<EventBase>).Assembly);
            session.AddReference(typeof(Func<int, bool>).Assembly);
            var actual = session.Execute<Func<int, bool>>("i => i == 1");
            actual.Should().NotBeNull("null compilation");
            actual.Invoke(2).Should().BeFalse("Should return false on 2");
            actual.Invoke(1).Should().BeTrue("Should return true on 1");
        }

        [TestMethod]
        public void Compiler_CustomType()
        {
            var session = new ScriptEngine().CreateSession();
            Type generic = typeof (Func<,>);
            var genericType = generic.MakeGenericType(typeof (EventBase), typeof (bool));
            session.AddReference(generic.Assembly);
            session.AddReference(typeof(EventBase).Assembly);
            var actual = session.Execute<Func<EventBase, bool>>("@event => @event.Type == 1");
            actual.Should().NotBeNull("null compilation");
            actual.Invoke(new Event<int>(1, 1)).Should().BeTrue();
            actual.Invoke(new Event<int>(2, 1)).Should().BeFalse();
        }

        [TestMethod]
        public void MostBasic_Count_100x()
        {
            // Where type = 1

            var testInterval = new TestInterval();

            var query = new QueryState<EventBase>();

            var session = ConfigureSession(query);
            session.Execute("Filters.Add(@event => @event.Type == 1);");

            int count = 0;
            testInterval.Interval.WhereEach(query.Filters).Count().Subscribe(i => count = i);
            testInterval.Start();
            count.Should().Be(testInterval.ExpectedCount / testInterval.NumberOfTypes);
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
