using System;
using System.Collections.Generic;

namespace RoslynRx.Tests
{
    public class QueryState<T>
    {
        public List<Func<T, bool>> Filters { get; private set; }

        public QueryState()
        {
            Filters = new List<Func<T, bool>>();
        }
    }
}