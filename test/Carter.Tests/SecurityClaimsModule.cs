namespace Carter.Tests
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;

    public class SecurityClaimsModule : CarterModule
    {
        public SecurityClaimsModule()
        {
            this.RequiresClaims(c => c.Type == ClaimTypes.Actor);

            this.Get("/secureclaim", async (request, response, routeData) => { await response.WriteAsync("secure claim"); });
        }
    }
}
