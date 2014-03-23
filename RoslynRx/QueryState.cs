using System;
using System.Collections.Generic;
using NLog;

namespace RoslynRx
{
    public class QueryState<T>
    {
        public List<Func<T, bool>> Filters { get; private set; }

        public QueryState()
        {
            Filters = new List<Func<T, bool>>();
        }
    }

    public class LogTimer : IDisposable
    {
        private string message;
        private Logger log;
        private DateTime start = DateTime.UtcNow;

        public LogTimer(string message, Logger log)
        {
            this.message = message;
            this.log = log;
        }

        public void Dispose()
        {
            var ellapsed = (DateTime.UtcNow - start).TotalMilliseconds;
            log.Info("T: {0} Took: {1} ms", message, ellapsed);    
        }
    }
}