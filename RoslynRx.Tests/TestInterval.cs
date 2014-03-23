using System;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;

namespace RoslynRx.Tests
{
    public class TestInterval
    {
        public int ExpectedCount;
        public int NumberOfTypes;
        public TestScheduler Scheduler;
        
        public TestInterval(int expectedCount = 60, int numberOfTypes = 2)
        {
            Scheduler = new TestScheduler();
            this.Interval = CreateTestSchedulerInterval(expectedCount, numberOfTypes, Scheduler);
            ExpectedCount = expectedCount;
            NumberOfTypes = numberOfTypes;
        }

        public void Start()
        {
            Scheduler.Start();
        }

        private static IObservable<Event<long>> CreateTestSchedulerInterval(int numberOfEvents, int numberOfTypes, TestScheduler scheduler)
        {
            return Observable.Interval(TimeSpan.FromSeconds(1), scheduler)
                .Take(numberOfEvents)
                .Select(i => new Event<long>((int)i % numberOfTypes, i));
        }

        public IObservable<Event<long>> Interval { get; private set; }
    }
}