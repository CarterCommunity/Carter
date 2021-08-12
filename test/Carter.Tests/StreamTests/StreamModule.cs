namespace Carter.Tests.StreamTests
{
    using System.IO;
    using System.Net.Mime;
    using System.Text;
    using Carter.Response;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class StreamModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/downloadwithcd", async (HttpResponse response) =>
            {
                using (var mystream = new MemoryStream(Encoding.ASCII.GetBytes("hi")))
                {
                    var cd = new ContentDisposition { FileName = "journal.csv" };
                    await response.FromStream(mystream, "application/csv", cd);
                }
            });

            app.MapGet("/download", async (HttpResponse response) =>
            {
                using (var mystream = new MemoryStream(Encoding.ASCII.GetBytes("hi")))
                {
                    await response.FromStream(mystream, "application/csv");
                }
            });

            app.MapGet("/downloadrange", async (HttpResponse response) =>
            {
                using (var mystream = new MemoryStream(Encoding.ASCII.GetBytes("0123456789")))
                {
                    await response.FromStream(mystream, "application/csv");
                }
            });
        }
    }
}
