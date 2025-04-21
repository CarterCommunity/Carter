namespace Carter.Tests
{
    using Microsoft.Extensions.Logging;
    using System;

    public class TUnitLogerProvider : ILoggerProvider
    {
        private readonly Action<TUnitLogger.Entry> sink;

        public TUnitLogerProvider( Action<TUnitLogger.Entry> sink = null)
        {
            this.sink = sink;
        }

        public ILogger CreateLogger(string categoryName)
            => new TUnitLogger(categoryName, sink);

        public void Dispose() { }
    }

    public class TUnitLogger : ILogger, IDisposable
    {
        private readonly string categoryName;
        private readonly Action<Entry> sink;

        public TUnitLogger(string categoryName, Action<Entry> sink = null)
        {
            this.categoryName = categoryName;
            this.sink = sink ?? (_ => { });
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var log = $"[{logLevel} {categoryName}] {formatter(state, exception)}";

            Console.WriteLine(log);

            sink(new EntryImpl
            {
                CategoryName = categoryName,
                LogLevel = logLevel,
                EventId = eventId,
                State = state,
                Exception = exception,
                Formatted = formatter(state, exception)
            });
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
        }

        public interface Entry
        {
            string CategoryName { get; }
            LogLevel LogLevel { get; }
            EventId EventId { get; }
            object State { get; }
            Exception Exception { get; }
            string Formatted { get; }
        }

        private class EntryImpl : Entry
        {
            public string CategoryName { get; set; }
            public LogLevel LogLevel { get; set; }
            public EventId EventId { get; set; }
            public object State { get; set; }
            public Exception Exception { get; set; }
            public string Formatted { get; set; }
        }
    }
}
