namespace CarterTemplate
{
    using Carter;
    using Microsoft.AspNetCore.Http;

    public class HomeModule : CarterModule
    {
        public HomeModule()
        {
            Get("/", async(req, res) => await res.WriteAsync("Hello from Carter!"));
        }
    }
}
