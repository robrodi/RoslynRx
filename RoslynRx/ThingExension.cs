using System;

namespace RoslynRx
{
    public static class ThingExension
    {
        public static IObservable<TEvent> DoThing<TEvent>(this IObservable<TEvent> stream, Predicate<TEvent> predicate)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (predicate == null) return stream;

            return predicate.GetAction().Invoke(stream);
        }
    }
}