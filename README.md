# Botwin

[![NuGet Version](http://img.shields.io/nuget/v/Botwin.svg?style=flat)](https://www.nuget.org/packages/Botwin/) 

Botwin is a library that allows [Nancy-esque](http://nancyfx.org) routing for use with ASP.Net Core. 

This is not a framework, it simply builds on top of [Microsoft.AspNetCore.Routing](https://github.com/aspnet/Routing) allowing you to have more elegant routing rather than have attribute routing, convention routing, ASP.Net Controllers or `IRouteBuilder` extensions. 

For a better understanding take a good look at the [samples](https://github.com/jchannon/Botwin/tree/master/samples) inside this repo.  The sample also demonstrates usages of elegant extensions around common ASP.Net Core types as shown below.  

Other extensions include:

* `Bind/BindAndValidate<T>` - [FluentValidation](https://github.com/JeremySkinner/FluentValidation) extensions to validate incoming HTTP requests.  
* Global `Before/After` hooks for every request
* `Before/After` hooks to the routes defined in a Botwin module
* Routes to use in common ASP.Net Core middleware eg. `app.UseExceptionHandler("/errorhandler");`.  
* `IStatusCodeHandler`s are also an option as the ASP.Net Core `UseStatusCodePages` middleware is not elegant enough IMO. `IStatusCodeHandler`s allow you to define what happens when one of your routes returns a specific status code.  An example usage is shown in the sample.
* `IResponseNegotiator`s allow you to define how the response should look on a certain Accept header.  Handling JSON is built in and the default response but implementing an interface allows the user to choose how they want to represent resources.
* All interface implementations are registered into ASP.Net Core DI automatically, implement the interface and off you go.
* Supports two different routing APIs 

  (i)
  ```
  this.Get("/actors/{id:int}", async (req, res, routeData) =>
  {
      var person = actorProvider.Get(routeData.AsInt("id"));
      await res.Negotiate(person);
  });
  ``` 
  (ii)
  ```
  this.Get("/actors/{id:int}", async (ctx) =>
  {
      var person = actorProvider.Get(ctx.GetRouteData().AsInt("id"));
      await ctx.Response.Negotiate(person);
  };
  ```


### Where does the name "Botwin" come from?

I have been a huge fan of and core contributor to [Nancy](http://nancyfx.org), the best .Net web framework, for many years and the name "Nancy" came about due to it being inspired from Sinatra the Ruby web framework.  Frank Sinatra had a daughter called Nancy and so that's where it came from.

I was also trying to think of a derivative name and I had recently been watching [Weeds](http://www.imdb.com/title/tt0439100/) and the lead character was called Nancy Botwin so this is where I got the name from! 

### Getting Started

A template for Botwin is out!

`dotnet new -i BotwinTemplate`

`dotnet new botwin -n MyBotwinApp`

`dotnet run`

Enjoy!

[https://www.nuget.org/packages/BotwinTemplate/](https://www.nuget.org/packages/BotwinTemplate/)

### Sample

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IActorProvider, ActorProvider>();
        services.AddBotwin();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseBotwin();
    }
}

public class ActorsModule : BotwinModule
{
    public ActorsModule(IActorProvider actorProvider)
    {
        this.Get("/actors", async (req, res, routeData) =>
        {
            var people = actorProvider.Get();
            await res.AsJson(people);
        });

        this.Get("/actors/{id:int}", async (req, res, routeData) =>
        {
            var person = actorProvider.Get(routeData.AsInt("id"));
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
