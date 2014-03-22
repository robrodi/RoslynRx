using System;
using System.Collections.Generic;
using System.Reactive.Linq;
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
    }
}
