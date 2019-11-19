namespace Carter.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.AspNetCore.Routing.Patterns;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using Microsoft.Net.Http.Headers;
    using Xunit;

    public class Test
    {
        public static RouteEndpointBuilder routes = new RouteEndpointBuilder(
            context => context.Response.WriteAsync(context.Request.Method.ToString()),
            RoutePatternFactory.Parse("/test"),
            0);
    }

    public class HelloModule : CarterModule2
    {
        public HelloModule()
        {
            //this.RequiresPolicy("secure");

            this.RequiresAuthentication();

            this.Get("/fowler/", context => context.Response.WriteAsync("Hello Fowler")); //.RequireAuthorization();
        }
    }

    public class CarterModule2
    {
        internal bool RequiresAuth { get; set; }

        internal string[] AuthPolicies { get; set; } = Array.Empty<string>();

        internal readonly Dictionary<(string verb, string path), (RequestDelegate handler, RouteConventions conventions)> Routes =
            new Dictionary<(string verb, string path), (RequestDelegate handler, RouteConventions conventions)>();

        public IEndpointConventionBuilder Get(string path, RequestDelegate handler)
        {
            var conventions = new RouteConventions();
            Routes.Add((HttpMethods.Get, path), (handler, conventions));
            return conventions;
        }

        internal class RouteConventions : IEndpointConventionBuilder
        {
            private readonly List<Action<EndpointBuilder>> _actions = new List<Action<EndpointBuilder>>();

            public void Add(Action<EndpointBuilder> convention)
            {
                _actions.Add(convention);
            }

            public void Apply(IEndpointConventionBuilder builder)
            {
                foreach (var a in _actions)
                {
                    builder.Add(a);
                }
            }
        }
    }

    public static class CarterExtensions2
    {
        public static IEndpointConventionBuilder MapCarter(this IEndpointRouteBuilder builder)
        {
            var appbuilder = builder.CreateApplicationBuilder();

            var builders = new List<IEndpointConventionBuilder>();

            using (var scope = builder.ServiceProvider.CreateScope())
            {
                foreach (var module in scope.ServiceProvider.GetServices<CarterModule2>())
                {
                    foreach (var route in module.Routes)
                    {
                        
                        var conventionBuilder = builder.MapMethods(route.Key.path, new[] { route.Key.verb }, async context =>
                        {
                            Func<HttpContext,Task> ff =  async (ctx) =>
                            {
                                await ctx.Response.WriteAsync("before");
                            };
                            
                            //await ff(context);
                            await route.Value.handler(context);
                        });

                        if (module.AuthPolicies.Any())
                        {
                            conventionBuilder.RequireAuthorization(module.AuthPolicies);
                        }
                        else if (module.RequiresAuth)
                        {
                            conventionBuilder.RequireAuthorization();
                        }

                        route.Value.conventions.Apply(conventionBuilder);
                        builders.Add(conventionBuilder);
                    }
                }
            }

            // Allow the user to apply conventions to all modules
            return new CompositeConventionBuilder(builders);
        }

        private class CompositeConventionBuilder : IEndpointConventionBuilder
        {
            private readonly List<IEndpointConventionBuilder> _builders;

            public CompositeConventionBuilder(List<IEndpointConventionBuilder> builders)
            {
                _builders = builders;
            }

            public void Add(Action<EndpointBuilder> convention)
            {
                foreach (var builder in _builders)
                {
                    builder.Add(convention);
                }
            }
        }
    }

    public static class Carter2Extensions
    {
        public static void RequiresPolicy(this CarterModule2 module, params string[] policyNames)
        {
            module.AuthPolicies = module.AuthPolicies.Concat(policyNames).ToArray();
        }

        public static void RequiresAuthentication(this CarterModule2 module)
        {
            module.RequiresAuth = true;
        }
    }

    public class CustomAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "custom auth";

        public string Scheme => DefaultScheme;

        public StringValues AuthKey { get; set; }
    }

    public static class AuthenticationBuilderExtensions
    {
        // Custom authentication extension method
        public static AuthenticationBuilder AddCustomAuth(this AuthenticationBuilder builder, Action<CustomAuthOptions> configureOptions)
        {
            // Add custom authentication scheme with custom options and custom handler
            return builder.AddScheme<CustomAuthOptions, CustomAuthHandler>(CustomAuthOptions.DefaultScheme, configureOptions);
        }
    }

    public class CustomAuthHandler : AuthenticationHandler<CustomAuthOptions>
    {
        public CustomAuthHandler(IOptionsMonitor<CustomAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Get Authorization header value
            if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
            {
                return Task.FromResult(AuthenticateResult.Fail("Cannot read authorization header."));
            }

            // The auth key from Authorization header check against the configured ones
            if (authorization.Any(key => Options.AuthKey.All(ak => ak != key)))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid auth key."));
            }

            // Create authenticated user
            var identities = new List<ClaimsIdentity> { new ClaimsIdentity(new[] { new Claim("claim", "you haz claim") }, "custom auth type") };
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), Options.Scheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class RawTests
    {
        private TestServer server;

        private HttpClient httpClient;

        public RawTests()
        {
            this.server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(x =>
                    {
                        x.AddRouting();
                        x.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = CustomAuthOptions.DefaultScheme;
                            options.DefaultChallengeScheme = CustomAuthOptions.DefaultScheme;
                        }).AddCustomAuth(options => options.AuthKey = "custom auth key");
                        //x.AddAuthorization(options => options.AddPolicy("secure", builder => builder.RequireClaim("claim")));
                        x.AddAuthorization();
                        x.AddScoped<CarterModule2, HelloModule>();
                    })
                    .Configure(x =>
                    {
                        //x.UseRouter(builder => builder.MapGet())
                        x.UseRouting();
                        x.UseAuthentication();
                        x.UseAuthorization();
                        
                        x.UseEndpoints(builder =>
                        {
                            builder.MapCarter();

                            // var dataSource = new DefaultEndpointDataSource(Test.routes.Build());
                            //
                            // builder.DataSources.Add(dataSource);
                            //
                            // builder.DataSources.Add(new DefaultEndpointDataSource(new RouteEndpointBuilder(
                            //     context => context.Response.WriteAsync("poop"),
                            //     RoutePatternFactory.Parse("/poop"),
                            //     0).Build()));
                            //
                            // builder.MapMethods("/", new[] { "GET", "HEAD" }, context =>
                            // {
                            //     context.Response.Headers["JON"] = "DEV";
                            //     return context.Response.WriteAsync("hi");
                            // });
                        });
                    }));
            this.httpClient = this.server.CreateClient();
        }

        [Fact]
        public async Task fowler()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/fowler");
            requestMessage.Headers.Authorization = AuthenticationHeaderValue.Parse("custom auth key");
            var response = await this.httpClient.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Hello Fowler", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task HEAD_TEST()
        {
            var response = await this.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, "/"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // With this commented out response.Content.Headers.ContentLength is null

            // var foo = await response.Content.ReadAsStringAsync();

            // With it uncommented response.Content.Headers.ContentLength == 2 on a HEAD request
            // Does reading it somehow cause the headers to be populated?

            Assert.Null(response.Content.Headers.ContentLength);
            Assert.True(response.Headers.Contains("JON"));
        }

        [Fact]
        public async Task GET_TEST()
        {
            var response = await this.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, response.Content.Headers.ContentLength);
        }

        [Fact]
        public async Task GET_TEST2()
        {
            var response = await this.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/test"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("GET", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task GET_TEST32()
        {
            var response = await this.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/poop"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("poop", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task NOT_ALLOWED_TEST()
        {
            var response = await this.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Put, "/"));
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }
    }
}
