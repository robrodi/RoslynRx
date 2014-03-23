using System;

namespace RoslynRx.Tests
{
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