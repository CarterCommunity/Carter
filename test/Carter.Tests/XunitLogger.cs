namespace Carter.Tests
{
    using Microsoft.Extensions.Logging;
    using System;
    using Xunit.Abstractions;

    public class XunitLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly Action<XunitLogger.Entry> sink;

        public XunitLoggerProvider(ITestOutputHelper testOutputHelper, Action<XunitLogger.Entry> sink = null)
        {
            this.testOutputHelper = testOutputHelper;
            this.sink = sink;
        }

        public ILogger CreateLogger(string categoryName)
            => new XunitLogger(categoryName, testOutputHelper, sink);

        public void Dispose() { }
    }

    public class XunitLogger : ILogger, IDisposable
    {
        private readonly string categoryName;
        private readonly ITestOutputHelper output;
        private readonly Action<Entry> sink;

        public XunitLogger(string categoryName, ITestOutputHelper output, Action<Entry> sink = null)
        {
            this.categoryName = categoryName;
            this.output = output;
            this.sink = sink ?? (_ => { });
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var log = $"[{logLevel} {categoryName}] {formatter(state, exception)}";

            output.WriteLine(log);

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
