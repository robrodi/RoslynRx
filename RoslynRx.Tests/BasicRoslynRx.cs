using System;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RoslynRx.Tests
{
    [TestClass]
    public class BasicRoslynRx
    {
        [TestMethod]
        public void MostBasic_Count()
        {
            var scheduler = new TestScheduler();
            var interval = Observable.Interval(TimeSpan.FromSeconds(1), scheduler).Take(60).Select(i => new Event<long>((int)i % 2, i));
            Func<EventBase, bool> filter = i => false;
            interval.Subscribe(Console.WriteLine);
            scheduler.Start();
            Assert.Inconclusive();
        }
    }

    public class Event<T> : EventBase
    {
        public Event(DateTime time, int type, T data) : base(time, type)
        {
            Data = data;
        }

        public T Data { get; private set; }

        public Event(int type, T data) : base(type)
        {
            Data = data;
        }
    }

    public abstract class EventBase
    {
        public DateTime Time  { get; private set; }

        public int Type { get; private set; }

        protected EventBase(int type) :this(DateTime.UtcNow, type)
        {
        }

        protected EventBase(DateTime time, int type)
        {
            Time = time;
            Type = type;
        }

        public override string ToString()
        {
            return string.Format("Time: {0}, Type: {1}", Time, Type);
        }
    }
}
