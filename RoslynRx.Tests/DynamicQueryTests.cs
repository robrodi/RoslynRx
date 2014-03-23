using System;
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

        [TestMethod]
        public void CompoundWhere()
        {
            var filter = new Filter<Event<long>>("@event => @event.Type == 1L && @event.Data > 30 && @event != null && @event.Data != 203123123");

            var testInterval = new TestInterval();

            int count = 0;
            testInterval.Interval.DoThing(filter).Count().Subscribe(i => count = i);
            testInterval.Start();
            count.Should().Be((testInterval.ExpectedCount - 30) / testInterval.NumberOfTypes);
        }

        [TestMethod]
        public void Sum()
        {
            // NOTE: fully qualifying the type name matters.  If this were a real thing, you could put in default namespaces.
            var summer = new Aggregate<Event<long>>("(sum, @event) => new RoslynRx.Tests.Event<long>(0, sum.Data + @event.Data)");
            var testInterval = new TestInterval();
            long sum = long.MinValue;

            testInterval.Interval.DoThing(summer).Subscribe(i => sum = i.Data);
            testInterval.Start();
            sum.Should().Be(1770);
        }

        [TestMethod]
        public void RunningSum()
        {
            // NOTE: fully qualifying the type name matters.  If this were a real thing, you could put in default namespaces.
            var summer = new Scan<Event<long>>("(sum, @event) => new RoslynRx.Tests.Event<long>(0, sum.Data + @event.Data)");
            var testInterval = new TestInterval();
            long sum = long.MinValue;

            testInterval.Interval.DoThing(summer).Subscribe(i => { sum = i.Data; Console.WriteLine(i.Data); });
            var oldSum = 0L;
            for (int i = 0; i < testInterval.ExpectedCount; i++)
            {
                Console.WriteLine("Advancing. {0} {1}", i, oldSum);
                testInterval.Scheduler.AdvanceBy(TimeSpan.TicksPerSecond);
                oldSum = oldSum + i;
                sum.Should().Be(oldSum);
            }

            sum.Should().Be(1770);
        }

        [TestMethod]
        public void GroupBy()
        {
            Assert.Inconclusive("NYI");
        }

        [TestMethod]
        public void LargerScript()
        {
            Assert.Inconclusive("NYI");
        }
    }
}
