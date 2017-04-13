# Botwin

Botwin is a library that allows [Nancy-esque](http://nancyfx.org) routing for use with ASP.Net Core. 

This is not a framework, it simply builds on top of [Microsoft.AspNetCore.Routing](https://github.com/aspnet/Routing) allowing you to have more elegant routing rather than have attribute routing, convention routing, ASP.Net Controllers or IRouteBuilder extensions. 

For a better understanding take a good look at the [samples](https://github.com/jchannon/Botwin/tree/master/samples) inside this repo.  The sample also demonstrates usages of elegant extensions around common ASP.Net Core types as shown below.  Also included are extensions that use [FluentValidation](https://github.com/JeremySkinner/FluentValidation) to validate incoming HTTP requests.  Along with all the HTTP verbs you can also execute Before and After hooks to the routes defined in a Botwin module as well as define routes to use in common ASP.Net Core middleware eg. `app.UseExceptionHandler("/errorhandler");`.  `IStatusCodeHandler`s are also an option as the ASP.Net Core `UseStatusCodePages` middleware is not elegant enogh IMO. `IStatusCodeHandler`s allow you to define what happens when one of your routes returns a specific status code.  An example usage is shown in the sample.


### Where does the name "Botwin" come from?

I have been a huge fan of and core contributor to [Nancy](http://nancyfx.org), the best .Net web framework, for many years and the name "Nancy" came about due to it being inspired from Sinatra the Ruby web framework.  Frank Sinatra had a daughter called Nancy and so that's where it came from.

I was also trying to think of a derivative name and I had recently been watching [Weeds](http://www.imdb.com/title/tt0439100/) and the lead character was called Nancy Botwin so this is where I got the name from! 

### Sample

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