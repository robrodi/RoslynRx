using System;

namespace RoslynRx
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ThingExtension
    {
        /// <summary>
        /// Plugs our Predicates into the Linq syntax.
        /// </summary>
        /// <typeparam name="TEvent">The type of event</typeparam>
        /// <param name="stream">the stream</param>
        /// <param name="predicate">the predicate</param>
        /// <returns>The resulting stream</returns>
        public static IObservable<TEvent> Link<TEvent>(this IObservable<TEvent> stream, Predicate<TEvent> predicate)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (predicate == null) return stream;

            return predicate.GetAction().Invoke(stream);
        }
    }
}