namespace Botwin.Tests
{
    using Botwin.Response;

    public class NegotiatorModule : BotwinModule
    {
        public NegotiatorModule()
        {
            this.Get("/negotiate", (req, res, routeData) => res.Negotiate(new { FirstName = "Jim" }));
        }
    }
}