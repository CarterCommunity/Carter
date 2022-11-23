using System;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace Carter.Tests;

public class AuthorizationTests : IDisposable
{
    private readonly ITestOutputHelper outputHelper;

    private TestServer server;

    private HttpClient httpClient;

    private bool defaultPolicyEvaluated;

    private bool specificPolicyEvaluated;

    public AuthorizationTests(ITestOutputHelper outputHelper) =>
        this.outputHelper = outputHelper;

    [Fact]
    public async Task Should_evaluate_default_policy_when_no_specific_policy_is_specified()
    {
        // Arrange
        BuildTestServer<DefaultAuthorizationTestModule>();

        // Act
        _ = await this.httpClient.GetAsync("/authorizedendpoint");

        // Assert
        Assert.True(defaultPolicyEvaluated);
        Assert.False(specificPolicyEvaluated);
    }

    [Fact]
    public async Task Should_evaluate_specific_policy_when_it_is_specified()
    {
        // Arrange
        BuildTestServer<SpecificPolicyAuthorizationTestModule>();

        // Act
        _ = await this.httpClient.GetAsync("/authorizedendpoint");

        // Assert
        Assert.True(specificPolicyEvaluated);
        Assert.False(defaultPolicyEvaluated);
    }

    /// <summary>
    /// Builds a test server with the module specified by the <typeparamref name="TModule"/> type parameter.
    /// </summary>
    /// <typeparam name="TModule">The type of Carter module to register.</typeparam>
    private void BuildTestServer<TModule>()
        where TModule : AuthorizationTestModuleBase
    {
        this.server = new TestServer(
            new WebHostBuilder()
                .ConfigureServices(x =>
                {
                    x.AddLogging(b =>
                    {
                        XUnitLoggerExtensions.AddXUnit((ILoggingBuilder)b, outputHelper, x => x.IncludeScopes = true);
                        b.SetMinimumLevel(LogLevel.Debug);
                    });

                    x.AddAuthentication().AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("TestScheme", options => { });
                    x.AddAuthorization(configure: c =>
                    {
                        c.DefaultPolicy = new AuthorizationPolicyBuilder("TestScheme")
                            .RequireAssertion(handler: c =>
                            {
                                this.defaultPolicyEvaluated = true;
                                return true;
                            })
                            .Build();

                        c.AddPolicy(SpecificPolicyAuthorizationTestModule.SpecificPolicy, new AuthorizationPolicyBuilder("TestScheme")
                            .RequireAssertion(handler: c =>
                            {
                                this.specificPolicyEvaluated = true;
                                return true;
                            })
                            .Build());
                    });

                    x.AddRouting();
                    x.AddCarter(configurator: c =>
                    {
                        c.WithModule<TModule>();
                    });
                })
                .Configure(x =>
                {
                    x.UseRouting();
                    x.UseAuthentication();
                    x.UseAuthorization();
                    x.UseEndpoints(builder => builder.MapCarter());
                })
            );

        this.httpClient = this.server.CreateClient();
    }

    public void Dispose()
    {
        this.httpClient?.Dispose();
        this.server?.Dispose();
    }

    /// <summary>
    /// See https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0#mock-authentication
    /// </summary>
    private class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
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


