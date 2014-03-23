using System;
using System.Reactive.Linq;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            Func<EventBase, bool> x = e => e.Type == 1;

            var session = new ScriptEngine().CreateSession(this);
            session.AddReference(this.GetType().Assembly);
            session.Execute("filter = e => e.Type == 1;");

            int count = 0;
            interval.Where(filter).Count().Subscribe(i => count = i);
            scheduler.Start();
            count.Should().Be(expectedCount / numberOfTypes);
        }
    }
}
