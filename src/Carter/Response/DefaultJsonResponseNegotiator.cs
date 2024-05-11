﻿namespace Carter.Response;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

public class DefaultJsonResponseNegotiator : IResponseNegotiator
{
    private readonly JsonSerializerOptions jsonSettings;

    public DefaultJsonResponseNegotiator(IOptions<JsonOptions> options = default)
    {
        this.jsonSettings = options?.Value.SerializerOptions ?? new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public bool CanHandle(MediaTypeHeaderValue accept)
    {
        return accept.MediaType.ToString().IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public async Task Handle<T>(HttpRequest req, HttpResponse res, T model, CancellationToken cancellationToken)
    {
        res.ContentType = "application/json; charset=utf-8";

        await JsonSerializer.SerializeAsync(res.Body, model, model == null ? typeof(object) : model.GetType(), this.jsonSettings, cancellationToken);
    }
}