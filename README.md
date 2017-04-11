# Botwin

Botwin is a library that allows [Nancy-esque](http://nancyfx.org) routing for use with ASP.Net Core. 

This is not a framework, it simply builds on top of [Microsoft.AspNetCore.Routing](https://github.com/aspnet/Routing) allowing you to have more elegant routing rather than have attribute routing, convention routing, ASP.Net Controllers or IRouteBuilder extensions. 

For a better understanding take a look at the [samples](https://github.com/jchannon/Botwin/tree/master/samples) inside this repo.  The samples also provide elegant extensions around common ASP.Net Core types as shown below.  Also included are extensions that use [FluentValidation](https://github.com/JeremySkinner/FluentValidation) to validate incoming HTTP requests.


```
public class ActorsModule : BotwinModule
{
    public ActorsModule(IActorProvider actorProvider)
    {
        this.Get("/actors", async (req, res, routeData) =>
        {
            var people = actorProvider.Get();
            await res.WriteAsync(JsonConvert.SerializeObject(people));
        });

        this.Get("/actors/{id:int}", async (req, res, routeData) =>
        {
            var person = actorProvider.Get(routeData.Values.AsInt("id"));
            await res.Negotiate(person);
        });

        this.Put("/actors/{id:int}", async (req, res, routeData) =>
        {
            var result = req.BindAndValidate<Actor>();

            if (!result.ValidationResult.IsValid)
            {
                res.StatusCode = 422;
                await res.Negotiate(result.ValidationResult.GetFormattedErrors());
                return;
            }

            //Update the user in your database

            res.StatusCode = 204;
        });

        this.Post("/actors", async (req, res, routeData) =>
        {
            var result = req.BindAndValidate<Actor>();

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
    }
}
```