namespace Carter.Tests.InternalRooms;

using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

internal static class NestedInternalRoomModuleWrapper
{
    internal class NestedInternalRoomModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/nested-room", (HttpResponse res) =>
            {
                res.StatusCode = 409;
                return Results.Text("There's no place like 127.0.0.1");
            });
        }
    }    
}

