namespace Carter.Tests.ContentNegotiation
{
    using Carter.Response;

    public class NegotiatorModule : CarterModule
    {
        public NegotiatorModule()
        {
            this.Get("/negotiate", (req, res) => res.Negotiate(new { FirstName = "Jim" }));
        }
    }
}
