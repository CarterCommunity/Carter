namespace CarterSample.Features.Actors
{
    using System.IO;
    using System.Threading.Tasks;
    using Carter;
    using Carter.ModelBinding;
    using Carter.Request;
    using Carter.Response;
    using CarterSample.Features.Actors.OpenApi;

    public class ActorsModule : CarterModule
    {
        public ActorsModule(IActorProvider actorProvider)
        {
            this.Get<GetActors>("/actors", (req, res) =>
            {
                var people = actorProvider.Get();
                return res.AsJson(people);
            });

            this.Get<GetActorById>("/actors/{id:int}", (req, res) =>
            {
                var person = actorProvider.Get(req.RouteValues.As<int>("id"));
                return res.Negotiate(person);
            });

            this.Put<UpdateActor>("/actors/{id:int}", async (req, res) =>
            {
                var result = await req.BindAndValidate<Actor>();

                if (!result.ValidationResult.IsValid)
                {
                    res.StatusCode = 422;
                    await res.Negotiate(result.ValidationResult.GetFormattedErrors());
                    return;
                }

                //Update the user in your database

                res.StatusCode = 204;
            });

            this.Post<AddActor>("/actors", async (req, res) =>
            {
                var result = await req.BindAndValidate<Actor>();

                if (!result.ValidationResult.IsValid)
                {
                    res.StatusCode = 422;
                    await res.Negotiate(result.ValidationResult.GetFormattedErrors());
                    return;
                }

                //Save the user in your database

                res.StatusCode = 201;
                await res.Negotiate(result.Data);
            });

            this.Delete<DeleteActor>("/actors/{id:int}", (req, res) =>
            {
                actorProvider.Delete(req.RouteValues.As<int>("id"));
                res.StatusCode = 204;
                return Task.CompletedTask;
            });

            this.Get("/actors/download", async (request, response) =>
            {
                using (var video = new FileStream("earth.mp4", FileMode.Open)) //24406813
                {
                    await response.FromStream(video, "video/mp4");
                }
            });

            this.Get<EmptyOpenApiMetaData>("/empty", (request, response) => Task.CompletedTask);

            this.Get<SampleMetaData>("/actors/sample", (request, response) => Task.CompletedTask);

            this.Post<NoValidatorMetaData>("/actors/sample", (request, response) => Task.CompletedTask);
        }
    }
}
