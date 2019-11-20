namespace CarterSample.Features.Graph
{
    using System.IO;
    using Carter;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.AspNetCore.Routing.Internal;

    public class GraphModule : CarterModule
    {
        private readonly DfaGraphWriter graphWriter;

        private readonly EndpointDataSource endpointDataSource;

        public GraphModule(DfaGraphWriter graphWriter, EndpointDataSource endpointDataSource)
        {
            this.Get("/graph", async context =>
            {
                //Write out a graphvz format of the app's routing and view here https://dreampuf.github.io/GraphvizOnline
                var sw = new StringWriter();
                graphWriter.Write(endpointDataSource, sw);
                await context.Response.WriteAsync(sw.ToString());
            });
        }
    }
}
