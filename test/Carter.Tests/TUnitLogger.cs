namespace Carter.Tests
{
    using Microsoft.Extensions.Logging;
    using System;

    public class TUnitLoggerProvider : ILoggerProvider
    {
        private readonly TestContext context;
        
        public TUnitLoggerProvider(TestContext context)
        {
            this.context = context;
        }

        public ILogger CreateLogger(string categoryName)
            => new TUnitLogger(categoryName, context);

        public void Dispose() { }
    }

    public class TUnitLogger : ILogger, IDisposable
    {
        private readonly string categoryName;

        private readonly TestContext testContext;
        
        public TUnitLogger(string categoryName, TestContext testContext)
        {
            this.categoryName = categoryName;
            this.testContext = testContext;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            testContext.GetDefaultLogger().Log(MapLogLevel(logLevel), state, exception, formatter);
        }

        private static TUnit.Core.Logging.LogLevel MapLogLevel(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => TUnit.Core.Logging.LogLevel.Trace,
                LogLevel.Debug => TUnit.Core.Logging.LogLevel.Debug,
                LogLevel.Information => TUnit.Core.Logging.LogLevel.Information,
                LogLevel.Warning => TUnit.Core.Logging.LogLevel.Warning,
                LogLevel.Error => TUnit.Core.Logging.LogLevel.Error,
                LogLevel.Critical => TUnit.Core.Logging.LogLevel.Critical,
                LogLevel.None => TUnit.Core.Logging.LogLevel.None,
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
            };
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
