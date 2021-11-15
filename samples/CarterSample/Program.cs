var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IGetActorsQuery, GetActorsQuery>();
builder.Services.AddSingleton<IGetActorByIdQuery, GetActorByIdQuery>();
builder.Services.AddSingleton<IDeleteActorCommand, DeleteActorCommand>();

builder.Services.AddCarter();

var app = builder.Build();

app.MapCarter();
app.Run();
