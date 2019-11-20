namespace Carter.Tests.Security
{
    using Microsoft.AspNetCore.Http;

    public class SecureMultiPolicyModule : CarterModule
    {
        public SecureMultiPolicyModule()
        {
            this.RequiresPolicy("reallysecurepolicy", "reallysecuresecondpolicy");

            this.Get("/securemultipolicy", async (request, response) => { await response.WriteAsync("secure"); });
        }
    }
}