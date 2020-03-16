namespace Carter.Tests.ModelBinding
{
    using Carter.ModelBinding;
    using FakeItEasy;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public class ModelBinderBaseTests
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly List<XunitLogger.Entry> logEntries = new List<XunitLogger.Entry>();
        private string method;
        private string path;
        private string returnValue;
        private HttpRequest request;

        public ModelBinderBaseTests(ITestOutputHelper output)
        {
            var provider = new XunitLoggerProvider(output, entry => logEntries.Add(entry));
            loggerFactory = LoggerFactory.Create(logging =>
            {
                logging.AddFilter(level => true);
                logging.AddProvider(provider);
            });
        }

        private IModelBinder Arrange(Func<HttpRequest, Task<string>> bind)
        {
            method = Guid.NewGuid().ToString();
            path = "/" + Guid.NewGuid().ToString();
            returnValue = Guid.NewGuid().ToString();

            request = A.Fake<HttpRequest>();
            A.CallTo(() => request.Method).Returns(method);
            A.CallTo(() => request.Path).Returns(path);

            return new FakeModelBinder(loggerFactory, async _ => await bind(_));
        }

        [Fact]
        public async Task Should_debug_log_when_binding()
        {
            var binder = Arrange(request => Task.FromResult(returnValue));

            var result = await binder.Bind<string>(request);

            Assert.Equal(returnValue, result);

            var logs = CheckStandardAssertions(logEntries, method, path);

            foreach (var log in logs)
            {
                Assert.Equal(LogLevel.Debug.ToString(), log["LogLevel"]);
            }
        }

        [Fact]
        public async Task Should_allow_fallback_to_default_for_expected_exception_type()
        {
            var binder = Arrange(request => 
                throw new ArgumentException("something went terribly, terribly wrong"));

            var result = await binder.Bind<string>(request);

            Assert.Equal(default, result);

            var logs = CheckStandardAssertions(logEntries, method, path);

            foreach (var log in logs)
            {
                Assert.Equal(LogLevel.Debug.ToString(), log["LogLevel"]);
            }
        }

        [Fact]
        public async Task Should_throw_for_unexpected_exception_type()
        {
            var binder = Arrange(request =>
                throw new InvalidOperationException("something went terribly, terribly wrong"));

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await binder.Bind<string>(request));

            var logs = CheckStandardAssertions(logEntries, method, path);

            Assert.Equal(LogLevel.Debug, logEntries[0].LogLevel);
            Assert.Equal(LogLevel.Error, logEntries[1].LogLevel);
        }

        private IReadOnlyList<IReadOnlyDictionary<string, string>> CheckStandardAssertions(IReadOnlyList<XunitLogger.Entry> entries, string method, string path)
        {
            var logs = logEntries
                .Select(x => new
                {
                    x.LogLevel,
                    x.CategoryName,
                    Values = ((IReadOnlyList<KeyValuePair<string, object>>)x.State)
                    .ToDictionary(x => x.Key, x => x.Value.ToString())
                })
                .Select(x =>
                {
                    x.Values.Add("LogLevel", x.LogLevel.ToString());
                    x.Values.Add("CategoryName", x.CategoryName);
                    return x.Values;
                })
                .ToList();

            foreach (var log in logs)
            {
                Assert.EndsWith(nameof(FakeModelBinder), log["CategoryName"]);
                Assert.Equal(method, log["Method"]);
                Assert.Equal(path, log["Path"]);
                Assert.Equal(typeof(string).FullName, log["Type"]);
            }

            return logs;
        }

        private class FakeModelBinder : ModelBinderBase
        {
            private readonly Func<HttpRequest, Task<object>> bind;

            public FakeModelBinder(ILoggerFactory loggerFactory, Func<HttpRequest, Task<object>> bind) : base(loggerFactory)
            {
                this.bind = bind;
                this.HandleExceptionWithDefaultValue<ArgumentException>();
            }

            protected override async Task<T> BindCore<T>(HttpRequest request)
            {
                return (T)await bind(request);
            }
        }
    }
}
