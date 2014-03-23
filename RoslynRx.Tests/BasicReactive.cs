using System;
using System.Collections.Generic;
using System.Reactive.Linq;
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
            var scheduler = new TestScheduler();
            int expectedCount = 60;
            var interval = Observable.Interval(TimeSpan.FromSeconds(1), scheduler).Take(expectedCount).Select(i => new Event<long>((int)i % 2, i));
            interval.Subscribe(Console.WriteLine);
            int count = 0;
            interval.Where(e => e.Type == 0).Count().Subscribe(i => count = i);
            scheduler.Start();
            count.Should().Be(expectedCount / 2);
        }
    }
}
