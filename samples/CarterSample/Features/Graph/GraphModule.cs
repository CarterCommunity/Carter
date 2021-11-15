namespace CarterSample.Features.Graph;

public class GraphModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/graph",
            async (DfaGraphWriter graphWriter, EndpointDataSource endpointDataSource, HttpContext context) =>
            {
                //Write out a graphvz format of the app's routing and view here https://dreampuf.github.io/GraphvizOnline
                var sw = new StringWriter();
                graphWriter.Write(endpointDataSource, sw);
                await context.Response.WriteAsync(sw.ToString());
            });
    }
}
