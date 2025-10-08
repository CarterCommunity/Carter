using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace Carter.Tests;

using Microsoft.Extensions.Hosting;

public class AuthorizationTests(ITestOutputHelper outputHelper) : IDisposable
{
    private TestServer server;

    [Fact]
    public async Task Should_contain_endpoint_with_default_authz_metadata()
    {
        // Arrange and act
        await BuildTestServer<DefaultAuthorizationTestModule>();

        // Assert
        var endpoint = server.Services
            .GetServices<EndpointDataSource>()
            .SelectMany(x => x.Endpoints)
            .OfType<RouteEndpoint>()
            .Single(x => x.RoutePattern.RawText == "/authorizedendpoint");

        var authorizeMetadata = endpoint
            .Metadata
            .GetRequiredMetadata<AuthorizeAttribute>();

        Assert.NotNull(authorizeMetadata);
        Assert.Null(authorizeMetadata.Policy);
    }

    [Fact]
    public async Task Should_contain_endpoint_with_specific_authz_metadata()
    {
        // Arrange and act
        await BuildTestServer<SpecificPolicyAuthorizationTestModule>();

        // Assert
        var endpoint = server.Services
            .GetServices<EndpointDataSource>()
            .SelectMany(x => x.Endpoints)
            .OfType<RouteEndpoint>()
            .Single(x => x.RoutePattern.RawText == "/authorizedendpoint");

        var authorizeMetadata = endpoint
            .Metadata
            .OfType<AuthorizeAttribute>()
            .ToList();

        Assert.NotEmpty(authorizeMetadata);
        Assert.Equivalent(
            authorizeMetadata.ConvertAll(x => x.Policy),
            new[]
            {
                SpecificPolicyAuthorizationTestModule.SpecificPolicyOne,
                SpecificPolicyAuthorizationTestModule.SpecificPolicyTwo
            });
    }

    /// <summary>
    /// Builds a test server with the module specified by the <typeparamref name="TModule"/> type parameter.
    /// </summary>
    /// <typeparam name="TModule">The type of Carter module to register.</typeparam>
    private async Task BuildTestServer<TModule>()
        where TModule : AuthorizationTestModuleBase
    {
        var host = new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseTestServer() // If using TestServer
                    .ConfigureServices(x =>
                    {
                        x.AddLogging(b =>
                        {
                            XUnitLoggerExtensions.AddXUnit((ILoggingBuilder)b, outputHelper,
                                x => x.IncludeScopes = true);
                            b.SetMinimumLevel(LogLevel.Debug);
                        });

                        x.AddRouting();
                        x.AddCarter(configurator: c => { c.WithModule<TModule>(); });
                    })
                    .Configure(x =>
                    {
                        x.UseRouting();
                        x.UseEndpoints(builder => builder.MapCarter());
                    });
            })
            .Build();

        await host.StartAsync();

        this.server = host.GetTestServer();
    }

    public void Dispose()
    {
        this.server?.Dispose();
    }

    /// <summary>
    /// See https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#mock-authentication
    /// </summary>
    private class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
