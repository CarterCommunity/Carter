using Carter;
using CarterSample.Features.Actors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IActorProvider, ActorProvider>();
builder.Services.AddCarter();

var app = builder.Build();

app.MapCarter();
app.Run();
