namespace Carter;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Carter.ModelBinding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public static class RouteExtensions
{
    private static async ValueTask<object> RouteHandler<T>(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var param = (T)context.Arguments.FirstOrDefault(x => x?.GetType() == typeof(T));

        var result = context.HttpContext.Request.Validate(param);
        if (!result.IsValid)
        {
            var problemDetailsErrors = new Dictionary<string, object> { { "errors", result.GetFormattedErrors() } };
            return Results.Problem(statusCode: 422, extensions: problemDetailsErrors);
        }

        return await next(context);
    }

    public static RouteHandlerBuilder MapPost<T>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Delegate handler)
    {
        return endpoints.MapPost(pattern, handler).AddEndpointFilter(async (context, next) => await RouteHandler<T>(context, next));
    }

    public static RouteHandlerBuilder MapPut<T>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Delegate handler)
    {
        return endpoints.MapPut(pattern, handler).AddEndpointFilter(async (context, next) => await RouteHandler<T>(context, next));
    }
}
