namespace Botwin.Samples
{
    using Microsoft.AspNetCore.Http;

    public class HooksModule : BotwinModule
    {
        public HooksModule()
        {
            this.Before = async (req, res, routeData) =>
            {
                res.StatusCode = 402;
                await res.WriteAsync("Pay up you filthy animal");
                return null;
            };

            this.Get("/hooks", async (req, res, routeData) => await res.WriteAsync("Can't catch me here"));

            this.After = async (req, res, routeData) => await res.WriteAsync("Don't forget you owe me big bucks!");
        }
    }
}