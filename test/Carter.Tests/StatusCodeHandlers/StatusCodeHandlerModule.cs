namespace Carter.Tests.StatusCodeHandlers
{
    using Microsoft.AspNetCore.Http;

    public class StatusCodeHandlerModule : CarterModule
    {
        public StatusCodeHandlerModule()
        {
            this.Get("/statushandler", async (req, res) =>
            {
                res.StatusCode = 418;
                await res.WriteAsync("Hello");
            });
        }
    }
}