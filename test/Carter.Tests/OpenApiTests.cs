namespace Carter.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Carter.OpenApi;
    using Microsoft.AspNetCore.Http;
    using Xunit;

    public class OpenApiTests
    {
        private class ExampleResponse
        {
            public IEnumerable<string> Dogs { get; set; } = new[] { "Labrador", "Golden Retriever", "Alsation" };
        }

        [Fact]
        public async Task Get_OpenAPI_Response()
        {
            var openApiDelegate = OpenApi.CarterOpenApi.BuildOpenApiResponse(
                new CarterOptions(openApiOptions: new OpenApiOptions("OpenAPI Docs", new[] { "http://localhost" }, new Dictionary<string, OpenApiSecurity>())),
                new Dictionary<(string verb, string path), RouteMetaData>()
                {
                    { ("GET", "/foo"), new RouteMetaData(responses: new[] { new RouteMetaDataResponse() }) }
                }
            );

            using (var body = new MemoryStream())
            {
                var ctx = new DefaultHttpContext();
                ctx.Response.Body = body;
                await openApiDelegate.Invoke(ctx);
                var responseBody = Encoding.UTF8.GetString(body.ToArray());
                Assert.Contains("Docs", responseBody);
            }
        }
    }
}
