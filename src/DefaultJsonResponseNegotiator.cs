namespace Botwin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;
    using Newtonsoft.Json;

    public class DefaultJsonResponseNegotiator : IResponseNegotiator
    {
        public bool CanHandle(IList<MediaTypeHeaderValue> accept)
        {
            return accept.Any(x => x.MediaType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public async Task Handle(HttpRequest req, HttpResponse res, object model)
        {
            res.ContentType = "application/json; charset=utf-8";
            await res.WriteAsync(JsonConvert.SerializeObject(model));
        }
    }
}