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

            this.Get("/hooks", handler: (req, res) => res.WriteAsync("Can't catch me here"));

            this.After = (ctx) => ctx.Response.WriteAsync("Don't forget you owe me big bucks!");
        }
    }
}