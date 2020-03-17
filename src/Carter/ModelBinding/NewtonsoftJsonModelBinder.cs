namespace Carter.ModelBinding
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public sealed class NewtonsoftJsonModelBinder : ModelBinderBase
    {
        private static readonly JsonSerializer TheJsonSerializer = new JsonSerializer();

        public NewtonsoftJsonModelBinder(ILoggerFactory loggerFactory)
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
            
            return Task.FromResult(TheJsonSerializer.Deserialize<T>(jsonTextReader));
        }
    }
}
