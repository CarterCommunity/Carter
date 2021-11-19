namespace Carter.Response;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class NewtonsoftJsonResponseNegotiator : IResponseNegotiator
{
    private readonly JsonSerializerSettings jsonSettings;

    public NewtonsoftJsonResponseNegotiator()
    {
        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        this.jsonSettings = new JsonSerializerSettings { ContractResolver = contractResolver, NullValueHandling = NullValueHandling.Ignore };
    }
    public bool CanHandle(MediaTypeHeaderValue accept)
    {
        return accept.MediaType.ToString().IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;
    }
    public Task Handle(HttpRequest req, HttpResponse res, object model, CancellationToken cancellationToken)
    {
        res.ContentType = "application/json; charset=utf-8";
        return res.WriteAsync(JsonConvert.SerializeObject(model, this.jsonSettings), cancellationToken);
    }
}