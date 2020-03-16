namespace Carter.Tests.ModelBinding.NewtonsoftBinding
{
    using System.IO;
    using System.Threading.Tasks;
    using Carter.ModelBinding;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class CustomModelBinder : ModelBinderBase
    {
        public CustomModelBinder(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        protected override Task<T> BindCore<T>(HttpRequest request)
        {
            var syncIOFeature = request.HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }

            using var streamReader = new StreamReader(request.Body);
            using var jsonTextReader = new JsonTextReader(streamReader);

            return Task.FromResult(
                JsonSerializer.Create(NewtonsoftJsonUtils.JsonSerializerSettings)
                    .Deserialize<T>(jsonTextReader));
        }
    }
}
