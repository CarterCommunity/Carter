# Carter

[![NuGet Version](http://img.shields.io/nuget/v/Carter.svg?style=flat)](https://www.nuget.org/packages/carter) 

<a href="https://join.slack.com/t/cartercommunity/shared_invite/enQtMzY2Nzc0NjU2MTgyLWY3M2Y2Yjk3NzViN2Y3YTQ4ZDA5NWFlMTYxMTIwNDFkMTc5YWEwMDFiOWUyM2Q4ZmY5YmRkODYyYTllZDViMmE"><img src="./slack.svg" width="140px"/></a>

Carter is a framework that is a thin layer of extension methods and functionality over ASP.NET Core allowing the code to be more explicit and most importantly more enjoyable.

Carter simply builds on top of ASP.NET Core allowing you to have more elegant routing rather than attribute routing, convention routing, or ASP.NET Controllers. 

For a better understanding, take a good look at the [samples](https://github.com/CarterCommunity/Carter/tree/master/samples) inside this repo. The samples demonstrate usages of elegant extensions around common ASP.NET Core types as shown below.  

Other extensions include:

* `Bind/BindAndValidate<T>` - [FluentValidation](https://github.com/JeremySkinner/FluentValidation) extensions to validate incoming HTTP requests.
* `BindFile/BindFiles/BindFileAndSave/BindFilesAndSave` - Allows you to easily get access to a file/files that has been uploaded. Alternatively you can call `BindFilesAndSave` and this will save it to a path you specify.
* `Before/After` hooks to the routes defined in a Carter module.
* Routes to use in common ASP.NET Core middleware e.g., `app.UseExceptionHandler("/errorhandler");`.
* `IStatusCodeHandler`s are also an option as the ASP.NET Core `UseStatusCodePages` middleware is not elegant enough IMO. `IStatusCodeHandler`s allow you to define what happens when one of your routes returns a specific status code.  An example usage is shown in the sample.
* `IResponseNegotiator`s allow you to define how the response should look on a certain Accept header. Handling JSON is built in the default response but implementing an interface allows the user to choose how they want to represent resources.
* All interface implementations for Carter components are registered into ASP.NET Core DI automatically. Implement the interface and off you go.
* Supports two different routing APIs.

  (i)
  ```csharp
  this.Get("/actors/{id:int}", async (req, res) =>
  {
      var person = actorProvider.Get(req.RouteValues.As<int>("id"));
      await res.Negotiate(person);
  });
  ``` 
  (ii)
  ```csharp
  this.Get("/actors/{id:int}", async (ctx) =>
  {
      var person = actorProvider.Get(ctx.Request.RouteValues.As<int>("id"));
      await ctx.Response.Negotiate(person);
  });
  ```
#### Endpoint Routing

Carter supports endpoint routing and all the extensions `IEndpointConventionBuilder` offers. For example you can define a route with authorization required like so:

```csharp
this.Get("/", (req, res) => res.WriteAsync("There's no place like 127.0.0.1")).RequireAuthorization();
```

  
### OpenApi

Carter supports OpenApi out of the box.  Simply call `/openapi` from your API and you will get a OpenApi JSON response.

To configure your routes for OpenApi simply supply the meta data class on your routes. For example:

```csharp
this.Get<GetActors>("/actors", async (req, res) =>
{
    var people = actorProvider.Get();
    await res.AsJson(people);
});
```

The meta data class is the way to document your routes and looks something like this:

```csharp
public class GetActors : RouteMetaData
{
    public override string Description { get; } = "Returns a list of actors";

    public override RouteMetaDataResponse[] Responses { get; } =
    {
        new RouteMetaDataResponse
        {
            Code = 200,
            Description = $"A list of {nameof(Actor)}s",
            Response = typeof(IEnumerable<Actor>)
        }
    };

    public override string Tag { get; } = "Actors";
}
```

### Where does the name "Carter" come from?

I have been a huge fan of, and core contributor to [Nancy](http://nancyfx.org), the best .NET web framework, for many years, and the name "Nancy" came about due to it being inspired from Sinatra the Ruby web framework. Frank Sinatra had a daughter called Nancy and so that's where it came from.

I was also trying to think of a derivative name, and I had recently listened to the song Empire State of Mind where Jay-Z declares he is the new Sinatra. His real name is Shaun Carter so I took Carter and here we are!

### CI Builds

If you'd like to try the latest builds from the master branch add `https://ci.appveyor.com/nuget/carterci` to your NuGet.config and pick up the latest and greatest version of Carter.

### Getting Started

You can get started using either the template or by adding the package manually to a new or existing application.

#### Template

[https://www.nuget.org/packages/CarterTemplate/](https://www.nuget.org/packages/CarterTemplate/)

1. Install the template - `dotnet new -i CarterTemplate`

2. Create a new application using template - `dotnet new Carter -n MyCarterApp`

3. Run the application - `dotnet run`

#### Package

[https://www.nuget.org/packages/Carter](https://www.nuget.org/packages/Carter)

1. Create a new empty ASP.NET Core application - `dotnet new web -n MyCarterApp`

2. Change into the new project location - `cd ./MyCarterApp`

3. Add Carter package - `dotnet add package carter`

4. Modify your Startup.cs to use Carter

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCarter();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(builder => builder.MapCarter());
    }
}
```

5. Create a new Module

```csharp
    public class HomeModule : CarterModule
    {
        public HomeModule()
        {
            Get("/", async (req, res) => await res.WriteAsync("Hello from Carter!"));
        }
    }
```

6. Run the application - `dotnet run`

### Sample

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IActorProvider, ActorProvider>();
        services.AddCarter();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(builder => builder.MapCarter());
    }
}

public class ActorsModule : CarterModule
{
    public ActorsModule(IActorProvider actorProvider)
    {
        this.Get("/actors", async (req, res) =>
        {
            var people = actorProvider.Get();
            await res.AsJson(people);
        });

        this.Get("/actors/{id:int}", async (req, res) =>
        {
            var person = actorProvider.Get(req.RouteValues.As<int>("id"));
            await res.Negotiate(person);
        });

        this.Put("/actors/{id:int}", async (req, res) =>
        {
            var result = req.BindAndValidate<Actor>();

            if (!result.ValidationResult.IsValid)
            {
                res.StatusCode = 422;
                await res.Negotiate(result.ValidationResult.GetFormattedErrors());
                return;
            }

            // Update the user in your database

            res.StatusCode = 204;
        });

        this.Post("/actors", async (req, res) =>
        {
            var result = req.BindAndValidate<Actor>();

            if (!result.ValidationResult.IsValid)
            {
                res.StatusCode = 422;
                await res.Negotiate(result.ValidationResult.GetFormattedErrors());
                return;
            }

            // Save the user in your database

            res.StatusCode = 201;
            await res.Negotiate(result.Data);
        });

    }
}
```

[More samples](https://github.com/CarterCommunity/Carter/tree/master/samples)
