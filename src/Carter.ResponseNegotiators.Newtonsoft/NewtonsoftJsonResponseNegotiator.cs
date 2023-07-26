namespace Carter.ResponseNegotiators.Newtonsoft;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Newtonsoft.Json;
using global::Newtonsoft.Json.Serialization;

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

/// <summary>
/// A Newtonsoft implementation of <see cref="IResponseNegotiator"/>
/// </summary>
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
    public Task Handle<T>(HttpRequest req, HttpResponse res, T model, CancellationToken cancellationToken)
    {
        res.ContentType = "application/json; charset=utf-8";
        return res.WriteAsync(JsonConvert.SerializeObject(model, this.jsonSettings), cancellationToken);
    }
}