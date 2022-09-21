namespace Carter.Tests;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Carter.Tests.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

public class RouteExtensionsTests
{
    private readonly TestServer server;

    private readonly HttpClient httpClient;

    public RouteExtensionsTests(ITestOutputHelper outputHelper)
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

                    x.AddSingleton<IDependency, Dependency>();

                    x.AddRouting();
                    x.AddCarter(configurator: c =>
                        {
                            c.WithModule<TestModule>();
                            c.WithValidator<TestModelValidator>();
                        }
                    );
                })
                .Configure(x =>
                {
                    x.UseRouting();
                    x.UseEndpoints(builder => builder.MapCarter());
                })
        );
        this.httpClient = this.server.CreateClient();
    }

    [Theory]
    [InlineData("POST")]
    [InlineData("PUT")]
    public async Task Should_return_422_on_validation_failure(string httpMethod)
    {
        var res = await this.httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(httpMethod), "/endpointfilter")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new TestModel()), Encoding.UTF8, "application/json")
        });
        Assert.Equal(HttpStatusCode.UnprocessableEntity, res.StatusCode);
    }

    [Theory]
    [InlineData("POST")]
    [InlineData("PUT")]
    public async Task Should_pick_type_to_validate_no_matter_of_delegate_position(string httpMethod)
    {
        var res = await this.httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(httpMethod), "/endpointfilter")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new TestModel()), Encoding.UTF8, "application/json")
        });
        Assert.Equal(HttpStatusCode.UnprocessableEntity, res.StatusCode);
    }

    [Theory]
    [InlineData("POST")]
    [InlineData("PUT")]
    public async Task Should_hit_route_if_validation_successful(string httpMethod)
    {
        var res = await this.httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(httpMethod), "/endpointfilter")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new TestModel{MyStringProperty = "hi", MyIntProperty = 123}), Encoding.UTF8, "application/json")
        });

        var body = await res.Content.ReadAsStringAsync();
        
        Assert.Equal(httpMethod, body);
    }
}

internal interface IDependency
{
    void DoSomething();
}

internal class Dependency : IDependency
{
    public void DoSomething()
    {
    }
}
