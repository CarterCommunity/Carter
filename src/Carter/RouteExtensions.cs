namespace Carter;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Carter.ModelBinding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public static class RouteExtensions
{
    public static RouteHandlerBuilder MapPost<T>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Delegate handler)
    {
        return endpoints.MapPost(pattern, handler).AddEndpointFilter(async (context, next) =>
        {
            var tdparam = context.GetArgument<T>(0);

            var result = ValidationExtensions.Validate<T>(context.HttpContext.Request, tdparam);
            if (!result.IsValid)
            {
                var problemDetailsErrors = new Dictionary<string, object>()
                    { { "errors", result.GetFormattedErrors() } };
                return Results.Problem(statusCode: 422, extensions: problemDetailsErrors);
            }

            return await next(context);
        });
    }

    public static RouteHandlerBuilder MapPut<T>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Delegate handler)
    {
        return endpoints.MapPut(pattern, handler).AddEndpointFilter(async (context, next) =>
        {
            var tdparam = context.GetArgument<T>(0);

            var result = context.HttpContext.Request.Validate(tdparam);
            if (!result.IsValid)
            {
                var problemDetailsErrors = new Dictionary<string, object>()
                    { { "errors", result.GetFormattedErrors() } };
                return Results.Problem(statusCode: 422, extensions: problemDetailsErrors);
            }

            return await next(context);
        });
    }
}