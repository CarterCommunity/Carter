namespace Carter
{
    using System;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;

    public class DefaultJsonResponseNegotiator : IResponseNegotiator
    {
        private readonly JsonSerializerOptions jsonSettings;

        public DefaultJsonResponseNegotiator()
        {
            this.jsonSettings = new JsonSerializerOptions { IgnoreNullValues = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        public bool CanHandle(MediaTypeHeaderValue accept)
        {
            return accept.MediaType.ToString().IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public async Task Handle(HttpRequest req, HttpResponse res, object model, CancellationToken cancellationToken)
        {
            res.ContentType = "application/json; charset=utf-8";

            await JsonSerializer.SerializeAsync(res.Body, model, model == null ? typeof(object) : model.GetType(), this.jsonSettings, cancellationToken);
        }
    }
}
