namespace Carter.Tests
{
    using Carter.Response;

    public class NegotiatorModule : CarterModule
    {
        public NegotiatorModule()
        {
            this.Get("/negotiate", (req, res, routeData) => res.Negotiate(new { FirstName = "Jim" }));
        }
    }
}
