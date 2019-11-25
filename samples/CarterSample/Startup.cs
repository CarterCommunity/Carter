namespace CarterSample
{
    using System.Collections.Generic;
    using Carter;
    using CarterSample.Features.Actors;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IActorProvider, ActorProvider>();

            services.AddCarter(options =>
            {
                options.OpenApi.DocumentTitle = "Carter <3 OpenApi";
                options.OpenApi.ServerUrls = new[] { "http://localhost:5000" };
                options.OpenApi.Securities = new Dictionary<string, OpenApiSecurity>
                {
                    { "BearerAuth", new OpenApiSecurity { BearerFormat = "JWT", Type = OpenApiSecurityType.http, Scheme = "bearer" } },
                    { "ApiKey", new OpenApiSecurity { Type = OpenApiSecurityType.apiKey, Name = "X-API-KEY", In = OpenApiIn.header } }
                };
                options.OpenApi.GlobalSecurityDefinitions = new[] { "BearerAuth" };
            });
        }

        public void Configure(IApplicationBuilder app, IConfiguration config)
        {
            var appconfig = new AppConfiguration();
            config.Bind(appconfig);

            app.UseExceptionHandler("/errorhandler");

            app.UseRouting();

            app.UseSwaggerUI(opt =>
            {
                opt.RoutePrefix = "openapi/ui";
                opt.SwaggerEndpoint("/openapi", "Carter OpenAPI Sample");
            });

            app.UseEndpoints(builder => builder.MapCarter());
        }
    }
}
