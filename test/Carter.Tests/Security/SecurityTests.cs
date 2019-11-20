namespace Carter.Tests.Security
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Xunit;

    public class TestAuthenticationOptions : AuthenticationSchemeOptions
    {
        public TestAuthenticationOptions()
        {
            this.Claims = new List<Claim>();
        }

        public const string Scheme = "TestScheme";

        public virtual ClaimsIdentity Identity { get; set; }

        public bool AuthUser { get; set; }

        public IEnumerable<Claim> Claims { get; set; }
    }

    public class TestAuthHandler : AuthenticationHandler<TestAuthenticationOptions>
    {
        public TestAuthHandler(IOptionsMonitor<TestAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identities = new List<ClaimsIdentity> { new ClaimsIdentity(this.Options.Claims, "TestAuthType") };
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), TestAuthenticationOptions.Scheme);
            if (this.Options.AuthUser)
            {
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail("Unit test is set to not auth user"));
            }
        }
    }

    public class SecurityTests
    {
        private HttpClient httpClient;

        private void ConfigureServer(bool authedUser = false, IEnumerable<Claim> claims = null)
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(x =>
                    {
                        x.AddAuthentication(options =>
                            {
                                options.DefaultAuthenticateScheme = TestAuthenticationOptions.Scheme;
                                options.DefaultChallengeScheme = TestAuthenticationOptions.Scheme;
                            })
                            .AddScheme<TestAuthenticationOptions, TestAuthHandler>(TestAuthenticationOptions.Scheme, options =>
                            {
                                options.AuthUser = authedUser;
                                options.Claims = claims;
                            });

                        x.AddAuthorization(options =>
                        {
                            options.AddPolicy("reallysecurepolicy", policy => { policy.RequireClaim(ClaimTypes.Actor); });
                            options.AddPolicy("reallysecuresecondpolicy", policy => { policy.RequireClaim(ClaimTypes.Email); });
                        });

                        x.AddCarter(configurator: c =>
                            c.WithModule<SecurityModule>()
                                .WithModule<SecureSinglePolicyModule>()
                                .WithModule<SecureMultiPolicyModule>()
                        );
                    })
                    .Configure(x =>
                    {
                        x.UseRouting();
                        x.UseAuthentication();
                        x.UseAuthorization();

                        // if (authedUser)
                        // {
                        //     x.Use(async (context, next) =>
                        //     {
                        //         var identity = new GenericIdentity("AuthedUser");
                        //         if (claims != null)
                        //         {
                        //             identity.AddClaims(claims);
                        //         }
                        //
                        //         context.User = new ClaimsPrincipal(identity);
                        //         await next();
                        //     });
                        // }

                        x.UseEndpoints(builder => builder.MapCarter());
                    })
            );
            this.httpClient = server.CreateClient();
        }

        [Fact]
        public async Task Should_return_200_if_user_authenticated()
        {
            //Given
            this.ConfigureServer(true);
            //When
            var response = await this.httpClient.GetAsync("/secure");
            var body = response.StatusCode;

            //Then
            Assert.Equal(200, (int)body);
        }

        [Fact]
        public async Task Should_return_401_when_current_user_not_set()
        {
            //Given
            this.ConfigureServer();

            //When
            var response = await this.httpClient.GetAsync("/secure");
            var body = response.StatusCode;

            //Then
            Assert.Equal(401, (int)body);
        }

        [Fact]
        public async Task Should_return_200_when_valid_policy()
        {
            //Given
            this.ConfigureServer(authedUser: true, new[] { new Claim(ClaimTypes.Actor, "Nicholas Cage") });

            //When
            var response = await this.httpClient.GetAsync("/securepolicy");
            var body = response.StatusCode;

            //Then
            Assert.Equal(200, (int)body);
        }

        [Fact]
        public async Task Should_return_403_when_invalid_policy()
        {
            //Given
            this.ConfigureServer(authedUser: true);

            //When
            var response = await this.httpClient.GetAsync("/securepolicy");

            //Then
            Assert.Equal(403, (int)response.StatusCode);
        }

        [Fact]
        public async Task Should_return_200_when_valid_on_multiple_policies()
        {
            //Given
            this.ConfigureServer(authedUser: true, new[] { new Claim(ClaimTypes.Actor, "Nicholas Cage"), new Claim(ClaimTypes.Email, "faceoff@niccage.com") });

            //When
            var response = await this.httpClient.GetAsync("/securemultipolicy");
            var body = response.StatusCode;

            //Then
            Assert.Equal(200, (int)body);
        }

        [Fact]
        public async Task Should_return_403_when_invalid_on_multiple_policies()
        {
            //Given
            this.ConfigureServer(authedUser: true, new[] { new Claim(ClaimTypes.Actor, "Nicholas Cage") });

            //When
            var response = await this.httpClient.GetAsync("/securemultipolicy");

            //Then
            Assert.Equal(403, (int)response.StatusCode);
        }
    }
}
