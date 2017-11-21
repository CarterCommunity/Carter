namespace Botwin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class DefaultJsonResponseNegotiator : IResponseNegotiator
    {
        private readonly JsonSerializerSettings jsonSettings;

        public DefaultJsonResponseNegotiator()
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            this.jsonSettings = new JsonSerializerSettings { ContractResolver = contractResolver };
        }

        public bool CanHandle(IList<MediaTypeHeaderValue> accept)
        {
            return accept.Any(x => x.MediaType.ToString().IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public async Task Handle(HttpRequest req, HttpResponse res, object model, CancellationToken cancellationToken)
        {
            res.ContentType = "application/json; charset=utf-8";
            await res.WriteAsync(JsonConvert.SerializeObject(model, this.jsonSettings), cancellationToken);
        }
    }
}
