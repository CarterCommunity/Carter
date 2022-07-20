namespace Carter.OpenApi;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

public static class OpenApiExtensions
{
    public static RouteHandlerBuilder IncludeInOpenApi(this RouteHandlerBuilder builder)
    {
        builder.Add(endpointBuilder => { endpointBuilder.Metadata.Add(new IncludeOpenApi()); });
        return builder;
    }
    
    public static RouteGroupBuilder IncludeInOpenApi(this RouteGroupBuilder builder)
    {
        builder.WithMetadata(new IncludeOpenApi());
        //builder.Add(endpointBuilder => { endpointBuilder.Metadata.Add(new IncludeOpenApi()); });
        return builder;
    }
}

public interface IIncludeOpenApi
{
}

public class IncludeOpenApi : IIncludeOpenApi
{
}
