using System;

namespace RoslynRx
{
    public static class ThingExension
    {
        public static IObservable<TEvent> DoThing<TEvent>(this IObservable<TEvent> stream, Thing<TEvent> thing)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (thing == null) return stream;

            return thing.GetAction().Invoke(stream);
        }
    }
}