using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RoslynRx.Tests
{
    [TestClass]
    public class BasicReactive
    {
        [TestMethod]
        public void ReactiveTestingSimplestCase()
        {
            var scheduler = new TestScheduler();
            var interval = Observable.Interval(TimeSpan.FromSeconds(1), scheduler).Take(5);
            var expectedValues = new long[] { 0, 1, 2, 3, 4 };
            var actualValues = new List<long>();
            interval.Subscribe(actualValues.Add);
            scheduler.Start();
            CollectionAssert.AreEqual(expectedValues, actualValues);
        }

        [TestMethod]
        public void Count()
        {
            var testInterval = new TestInterval();

            int count = 0;
            testInterval.Interval.Where(e => e.Type == 0).Count().Subscribe(i => count = i);
            testInterval.Start();
            count.Should().Be(testInterval.ExpectedCount / testInterval.NumberOfTypes);
        }

        [TestMethod]
        public void Max()
        {
            var testInterval = new TestInterval();
            long max = long.MinValue;
            testInterval.Interval.Max(e => e.Data).Subscribe(i => max = i);
            testInterval.Start();
            max.Should().Be(testInterval.ExpectedCount - 1);
        }

        [TestMethod]
        public void Sum()
        {
            var testInterval = new TestInterval();
            long max = long.MinValue;
            testInterval.Interval.Aggregate((sum, @event) => new Event<long>(0, sum.Data + @event.Data)).Subscribe(i => max = i.Data);
            testInterval.Start();
            max.Should().Be(1770);
        }

        [TestMethod]
        public void GroupBy_Count()
        {
            var testInterval = new TestInterval();
            testInterval.Interval.GroupBy(@event => @event.Type).Subscribe(x =>
            {
                int count = 0;
                x.Subscribe(_ => Console.WriteLine("-> {0} : {1}", x.Key.ToString().PadLeft(3), (++count).ToString().PadLeft(3)));
            });

            testInterval.Start();
        }

        [TestMethod]
        public void Ding()
        {
            var testInterval = new TestInterval();
            long ding = long.MinValue;
            testInterval.Interval.SkipWhile(@event => @event.Data < 30).Take(1).Subscribe(@e => ding = @e.Data);
            testInterval.Start();
            ding.Should().Be(30);
        }
    }
}
