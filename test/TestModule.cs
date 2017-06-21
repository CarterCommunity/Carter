namespace Botwin.Tests
{
    using Microsoft.AspNetCore.Http;
    public class TestModule : BotwinModule
    {
        public TestModule()
        {
            this.Before = async (req, res, routeData) => { await res.WriteAsync("Before"); return res; };
            this.After = async (req, res, routeData) => { await res.WriteAsync("After"); };
            this.Get("/", async (request, response, routeData) => { await response.WriteAsync("Hello"); });
            this.Post("/", async (request, response, routeData) => { await response.WriteAsync("Hello"); });
            this.Put("/", async (request, response, routeData) => { await response.WriteAsync("Hello"); });
            this.Delete("/", async (request, response, routeData) => { await response.WriteAsync("Hello"); });
            this.Head("/head", async (request, response, routeData) => { await response.WriteAsync("Hello"); });
            this.Patch("/", async (request, response, routeData) => { await response.WriteAsync("Hello"); });
            this.Options("/", async (request, response, routeData) => { await response.WriteAsync("Hello"); });
        }
    }
}
