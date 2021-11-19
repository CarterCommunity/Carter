namespace Carter.OpenApi;

using Microsoft.AspNetCore.Builder;

public static class OpenApiExtensions
{
    public static RouteHandlerBuilder IncludeInOpenApi(this RouteHandlerBuilder builder)
    {
        builder.Add(endpointBuilder => { endpointBuilder.Metadata.Add(new IncludeOpenApi()); });
        return builder;
    }
}

public interface IIncludeOpenApi
{
}

public class IncludeOpenApi : IIncludeOpenApi
{
}
