using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RoslynRx.Tests
{
    [TestClass]
    public class DynamicQueryTests
    {
        [TestMethod]
        public void SimpleTest()
        {
            var filter = new Filter<Event<long>>("@event => @event.Type == 1L");

            var testInterval = new TestInterval();

            int count = 0;
            filter.GetAction().Invoke(testInterval.Interval).Count().Subscribe(i => count = i);
            testInterval.Start();
            count.Should().Be(testInterval.ExpectedCount / testInterval.NumberOfTypes);
        }

        [TestMethod]
        public void SimpleTest_WithExtension()
        {
            var filter = new Filter<Event<long>>("@event => @event.Type == 1L");

            var testInterval = new TestInterval();

            int count = 0;
            testInterval.Interval.DoThing(filter).Count().Subscribe(i => count = i);
            testInterval.Start();
            count.Should().Be(testInterval.ExpectedCount / testInterval.NumberOfTypes);
        }
    }
}
