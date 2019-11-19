namespace Carter.Tests.Security
{
    using Microsoft.AspNetCore.Http;

    public class SecurityModule : CarterModule
    {
        public SecurityModule()
        {
            this.RequiresAuthorization();

            this.Get("/secure", async (request, response) => { await response.WriteAsync("secure"); });
        }
    }
}
