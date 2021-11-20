using Carter.OpenApi;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IGetActorsQuery, GetActorsQuery>();
builder.Services.AddSingleton<IGetActorByIdQuery, GetActorByIdQuery>();
builder.Services.AddSingleton<IDeleteActorCommand, DeleteActorCommand>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Description = "Carter Sample API",
        Version = "v1",
        Title = "A Carter API to manage Actors/Films/Crew etc"
    });
    
    options.DocInclusionPredicate((s, description) =>
    {
        foreach (var metaData in description.ActionDescriptor.EndpointMetadata)
        {
            if (metaData is IIncludeOpenApi)
            {
                return true;
            }
        }
        return false;
    });
});

builder.Services.AddCarter();


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapCarter();

app.Run();

//Needed for testing unless you use InternalsVisibleTo in csproj
public partial class Program { }
