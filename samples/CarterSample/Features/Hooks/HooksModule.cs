namespace CarterSample.Features.Hooks
{
    using Carter;
    using Microsoft.AspNetCore.Http;

    public class HooksModule : CarterModule
    {
        public HooksModule()
        {
            this.Before = async (ctx) =>
            {
                ctx.Response.StatusCode = 402;
                await ctx.Response.WriteAsync("Pay up you filthy animal");
                return false;
            };

            this.Get("/hooks", async (req, res, routeData) => await res.WriteAsync("Can't catch me here"));

            this.After = async (ctx) => await ctx.Response.WriteAsync("Don't forget you owe me big bucks!");
        }
    }
}