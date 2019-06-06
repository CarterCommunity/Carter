namespace Carter.Tests.Security
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class SecurityTests
    {
        private HttpClient httpClient;

        private void ConfigureServer(bool authedUser = false, IEnumerable<Claim> claims = null)
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(x =>
                    {
                        x.AddAuthorization(options =>
                        {
                            options.AddPolicy("reallysecurepolicy", policy => { policy.RequireClaim(ClaimTypes.Actor); });
                            options.AddPolicy("reallysecuresecondpolicy", policy => { policy.RequireClaim(ClaimTypes.Email); });
                        });

                        x.AddCarter(configurator: c =>
                            c.WithModule<SecurityClaimsModule>()
                                .WithModule<SecurityModule>()
                                .WithModule<SecureSinglePolicyModule>()
                                .WithModule<SecureMultiPolicyModule>()
                        );
                    })
                    .Configure(x =>
                    {
                        if (authedUser)
                        {
                            x.Use(async (context, next) =>
                            {
                                var identity = new GenericIdentity("AuthedUser");
                                if (claims != null)
                                {
                                    identity.AddClaims(claims);
                                }

                                context.User = new ClaimsPrincipal(identity);
                                await next();
                            });
                        }

                        x.UseCarter();
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
        public async Task Should_return_200_when_valid_claims()
        {
            //Given
            this.ConfigureServer(true, new[] { new Claim(ClaimTypes.Actor, "Christian Slater") });

            //When
            var response = await this.httpClient.GetAsync("/secureclaim");
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
        public async Task Should_return_401_when_invalid_claims()
        {
            //Given
            this.ConfigureServer(true, new[] { new Claim(ClaimTypes.Thumbprint, "Zebra") });

            //When
            var response = await this.httpClient.GetAsync("/secureclaim");
            var body = response.StatusCode;

            //Then
            Assert.Equal(401, (int)body);
        }

        [Fact]
        public async Task Should_return_401_when_no_claims()
        {
            //Given
            this.ConfigureServer(true);

            //When
            var response = await this.httpClient.GetAsync("/secureclaim");
            var body = response.StatusCode;

            //Then
            Assert.Equal(401, (int)body);
        }

        [Fact]
        public async Task Should_return_401_when_not_authed_user_but_module_requires_claims()
        {
            //Given
            this.ConfigureServer();

            //When
            var response = await this.httpClient.GetAsync("/secureclaim");
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
        public async Task Should_return_401_when_invalid_policy()
        {
            //Given
            this.ConfigureServer(authedUser: true);

            //When
            var response = await this.httpClient.GetAsync("/securepolicy");
            var body = response.StatusCode;

            //Then
            Assert.Equal(401, (int)body);
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
        public async Task Should_return_401_when_invalid_on_multiple_policies()
        {
            //Given
            this.ConfigureServer(authedUser: true, new[] { new Claim(ClaimTypes.Actor, "Nicholas Cage") });

            //When
            var response = await this.httpClient.GetAsync("/securemultipolicy");
            var body = response.StatusCode;

            //Then
            Assert.Equal(401, (int)body);
        }

    }
}
