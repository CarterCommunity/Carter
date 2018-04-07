namespace Carter.Tests
{
    using Microsoft.AspNetCore.Http;

    public class SecurityModule : CarterModule
    {
        public SecurityModule()
        {
            this.RequiresAuthentication();

            this.Get("/secure", async (request, response, routeData) => { await response.WriteAsync("secure"); });
        }
    }
}
