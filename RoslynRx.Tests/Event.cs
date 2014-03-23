using System;

namespace RoslynRx.Tests
{
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

        public override string ToString()
        {
            return string.Format("{0}, Data: {1}", base.ToString(), Data);
        }
    }
}