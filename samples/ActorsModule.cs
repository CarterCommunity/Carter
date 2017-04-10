namespace Botwin.Samples
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public class ActorsModule : BotwinModule
    {
        public ActorsModule(IPersonProvider personProvider)
        {
            this.Get("/actors", async (req, res, routeData) =>
            {
                var people = personProvider.Get();
                await res.WriteAsync(JsonConvert.SerializeObject(people));
            });

            this.Get("/actors/{id:int}", async (req, res, routeData) =>
            {
                var person = personProvider.Get(routeData.Values.AsInt("id"));
                await res.Negotiate(person);
            });
        }
    }
}