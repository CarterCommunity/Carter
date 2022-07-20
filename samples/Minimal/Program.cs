var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var group = app.MapGroup("/actors").RequireAuthorization();
group.MapGet("/", () => "actors");

var group2 = app.MapGroup("/actors");
group2.MapGet("/tc", () => "tc");

app.MapGet("/", () => "Hello World!");

app.Run();
