namespace Carter.Tests.InternalRooms;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

internal class InternalRoomModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", (HttpResponse res) =>
        {
            res.StatusCode = 409;
            return Results.Text("There's no place like 127.0.0.1");
        });
    }
}
