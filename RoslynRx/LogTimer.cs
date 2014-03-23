using System;
using NLog;

namespace RoslynRx
{
    /// <summary>
    /// Some nLog hackiness.
    /// </summary>
    public class LogTimer : IDisposable
    {
        private string message;
        private Logger log;
        private DateTime start = DateTime.UtcNow;

        private bool disposed;
        public LogTimer(string message, Logger log)
        {
            this.message = message;
            this.log = log;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || disposed) return;
            var ellapsed = (DateTime.UtcNow - start).TotalMilliseconds;
            log.Info("T: {0} Took: {1} ms", message, ellapsed);
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LogTimer()
        {
            Dispose(false);
        }
    }
}