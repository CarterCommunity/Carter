namespace Carter.Tests
{
    using System.IO;
    using System.Net.Mime;
    using System.Text;
    using Carter.Response;

    public class StreamModule : CarterModule
    {
        public StreamModule()
        {
            this.Get("/downloadwithcd", async (request, response, routeData) =>
            {
                using (var mystream = new MemoryStream(Encoding.ASCII.GetBytes("hi")))
                {
                    var cd = new ContentDisposition { FileName = "journal.csv" };
                    await response.FromStream(mystream, "application/csv", cd);
                }
            });

            this.Get("/download", async (request, response, routeData) =>
            {
                using (var mystream = new MemoryStream(Encoding.ASCII.GetBytes("hi")))
                {
                    await response.FromStream(mystream, "application/csv");
                }
            });

            this.Get("/downloadrange", async (request, response, routeData) =>
            {
                using (var mystream = new MemoryStream(Encoding.ASCII.GetBytes("0123456789")))
                {
                    await response.FromStream(mystream, "application/csv");
                }
            });
        }
    }
}
