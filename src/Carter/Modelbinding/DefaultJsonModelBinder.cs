namespace Carter.ModelBinding
{
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public sealed class DefaultJsonModelBinder : ModelBinderBase
    {
        public DefaultJsonModelBinder(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            HandleExceptionWithDefaultValue<JsonException>();
        }

        protected override async  Task<T> BindCore<T>(HttpRequest request)
        {
            return await JsonSerializer.DeserializeAsync<T>(request.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
