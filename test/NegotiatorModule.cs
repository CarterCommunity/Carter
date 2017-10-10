namespace Botwin.Tests
{
    using Botwin.Extensions;

    public class NegotiatorModule : BotwinModule
    {
        public NegotiatorModule()
        {
            this.Get("/negotiate", (req, res, routeData) => res.Negotiate(new { Name = "Jim" }));
        }
    }
}