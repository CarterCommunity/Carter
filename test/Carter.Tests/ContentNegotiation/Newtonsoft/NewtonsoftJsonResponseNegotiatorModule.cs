namespace Carter.Tests.ContentNegotiation.Newtonsoft
{
    using Carter.Response;

    public class NewtonsoftJsonResponseNegotiatorModule : CarterModule
    {
        public NewtonsoftJsonResponseNegotiatorModule()
        {
            this.Get("/negotiate", (req, res) => res.Negotiate(new { FirstName = "Jim" }));
        }
    }
}
