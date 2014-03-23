using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace RoslynRx.Tests
{
    public static class RxEx
    {
        public static IObservable<TSource> WhereEach<TSource>(this IObservable<TSource> source, IEnumerable<Func<TSource, bool>> predicates)
        {
            IObservable<TSource> result = source;
            foreach (var func in predicates)
            {
                result = source.Where(func);
            }

            return result;
        }
    }
}