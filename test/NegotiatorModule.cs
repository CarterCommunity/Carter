namespace Botwin.Tests
{
    public class NegotiatorModule : BotwinModule
    {
        public NegotiatorModule()
        {
            this.Get("/negotiate", (req, res, routeData) => res.Negotiate(new { Name = "Jim" }));
        }
    }
}