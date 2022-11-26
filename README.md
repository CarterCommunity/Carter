# Carter

Carter is a framework that is a thin layer of extension methods and functionality over ASP.NET Core allowing the code to be more explicit and most importantly more enjoyable.

For a better understanding, take a good look at the [samples](https://github.com/CarterCommunity/Carter/tree/master/samples) inside this repo. The samples demonstrate usages of elegant extensions around common ASP.NET Core types as shown below.  

Other extensions include:

* `Validate<T>` - [FluentValidation](https://github.com/JeremySkinner/FluentValidation) extensions to validate incoming HTTP requests which is not available with ASP.NET Core Minimal APIs.
* `BindFile/BindFiles/BindFileAndSave/BindFilesAndSave` - Allows you to easily get access to a file/files that has been uploaded. Alternatively you can call `BindFilesAndSave` and this will save it to a path you specify.
* Routes to use in common ASP.NET Core middleware e.g., `app.UseExceptionHandler("/errorhandler");`.
* `IResponseNegotiator`s allow you to define how the response should look on a certain Accept header(content negotiation). Handling JSON is built in the default response but implementing an interface allows the user to choose how they want to represent resources.
* All interface implementations for Carter components are registered into ASP.NET Core DI automatically. Implement the interface and off you go.

### Releases

* Latest NuGet Release [![NuGet Version](http://img.shields.io/nuget/v/Carter.svg?style=flat)](https://www.nuget.org/packages/carter) 
* Latest NuGet Pre-Release [![NuGet Version](http://img.shields.io/nuget/vpre/Carter.svg?style=flat)](https://www.nuget.org/packages/carter) 
* Lateset CI Release [![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https%3A%2F%2Ff.feedz.io%2Fcarter%2Fcarter%2Fshield%2FCarter%2Flatest)](https://f.feedz.io/carter/carter/packages/Carter/latest/download) 
* Build Status [![Build Status](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Factions-badge.atrox.dev%2FCarterCommunity%2FCarter%2Fbadge%3Fref%3Dmain&style=flat)](https://actions-badge.atrox.dev/CarterCommunity/Carter/goto?ref=main)

### Join our Slack Channel

<a href="https://join.slack.com/t/cartercommunity/shared_invite/enQtMzY2Nzc0NjU2MTgyLWY3M2Y2Yjk3NzViN2Y3YTQ4ZDA5NWFlMTYxMTIwNDFkMTc5YWEwMDFiOWUyM2Q4ZmY5YmRkODYyYTllZDViMmE"><img src="./slack.svg" width="140px"/></a>

#### Routing

Carter uses `IEndpointRouteBuilder` routing and all the extensions `IEndpointConventionBuilder` offers also known as Minimal APIs. For example you can define a route with authorization required like so:

```csharp
app.MapGet("/", () => "There's no place like 127.0.0.1").RequireAuthorization();
```


### Where does the name "Carter" come from?

I have been a huge fan of, and core contributor to [Nancy](http://nancyfx.org), the best .NET web framework, for many years, and the name "Nancy" came about due to it being inspired from Sinatra the Ruby web framework. Frank Sinatra had a daughter called Nancy and so that's where it came from.

I was also trying to think of a derivative name, and I had recently listened to the song Empire State of Mind where Jay-Z declares he is the new Sinatra. His real name is Shaun Carter so I took Carter and here we are!

### CI Builds

If you'd like to try the latest builds from the master branch add `https://f.feedz.io/carter/carter/nuget/index.json` to your NuGet.config and pick up the latest and greatest version of Carter.

### Getting Started

You can get started using either the template or by adding the package manually to a new or existing application.

#### Template

[https://www.nuget.org/packages/CarterTemplate/](https://www.nuget.org/packages/CarterTemplate/)

1. Install the template - `dotnet new install CarterTemplate`

2. Create a new application using template - `dotnet new carter -n MyCarterApp -o MyCarterApp`

3. Go into the new directory created for the application `cd MyCarterApp`

4. Run the application - `dotnet run`

#### Package

[https://www.nuget.org/packages/Carter](https://www.nuget.org/packages/Carter)

1. Create a new empty ASP.NET Core application - `dotnet new web -n MyCarterApp`

2. Change into the new project location - `cd ./MyCarterApp`

3. Add Carter package - `dotnet add package carter`

4. Modify your Program.cs to use Carter

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCarter();

var app = builder.Build();

app.MapCarter();
app.Run();
```

5. Create a new Module

```csharp
    public class HomeModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/", () => "Hello from Carter!");
        }
    }
```

6. Run the application - `dotnet run`

### Sample

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IActorProvider, ActorProvider>();
builder.Services.AddCarter();

var app = builder.Build();

app.MapCarter();
app.Run();

public class HomeModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => "Hello from Carter!");
        app.MapGet("/qs", (HttpRequest req) =>
        {
            var ids = req.Query.AsMultiple<int>("ids");
            return $"It's {string.Join(",", ids)}";
        });
        app.MapGet("/conneg", (HttpResponse res) => res.Negotiate(new { Name = "Dave" }));
        app.MapPost("/validation", HandlePost);
    }

    private IResult HandlePost(HttpContext ctx, Person person, IDatabase database)
    {
        var result = ctx.Request.Validate(person);

        if (!result.IsValid)
        {
            return Results.UnprocessableEntity(result.GetFormattedErrors());
        }

        var id = database.StorePerson(person);

        ctx.Response.Headers.Location = $"/{id}";
        return Results.StatusCode(201);
    }
}

public record Person(string Name);

public interface IDatabase
{
    int StorePerson(Person person);
}

public class Database : IDatabase
{
    public int StorePerson(Person person)
    {
        //db stuff
    }
}
```

[More samples](https://github.com/CarterCommunity/Carter/tree/master/samples)

### Configuration

As mentioned earlier Carter will scan for implementations in your app and register them for DI. However, if you want a more controlled app, Carter comes with a `CarterConfigurator` that allows you to register modules, validators and response negotiators manually.

Carter will use a response negotiator based on `System.Text.Json`, though it provides for custom implementations via the `IResponseNegotiator` interface. To use your own implementation of `IResponseNegotiator` (say, `CustomResponseNegotiator`), add the following line to the initial Carter configuration, in this case as part of `Program.cs`:

```csharp

    builder.Services.AddCarter(configurator: c =>
    {
        c.WithResponseNegotiator<CustomResponseNegotiator>();
        c.WithModule<MyModule>();
        c.WithValidator<TestModelValidator>()
    });

```

Here again, Carter already ships with a response negotiator using `Newtonsoft.Json`, so you can wire up the Newtonsoft implementation with the following line:

```csharp
    builder.Services.AddCarter(configurator: c =>
    {
        c.WithResponseNegotiator<NewtonsoftJsonResponseNegotiator>();
    });
```
