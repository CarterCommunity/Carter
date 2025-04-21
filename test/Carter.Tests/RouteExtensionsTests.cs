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

public class RouteExtensionsTests
{
    private readonly TestServer server;

    private readonly HttpClient httpClient;

    public RouteExtensionsTests()
    {
        this.server = new TestServer(
            new WebHostBuilder()
                .ConfigureServices(x =>
                {
                    x.AddLogging(b =>
                    {
                        b.SetMinimumLevel(LogLevel.Debug);
                    });

                    x.AddSingleton<IDependency, Dependency>();

                    x.AddRouting();
                    x.AddCarter(configurator: c =>
                    {
                        c.WithModule<TestModule>();
                        c.WithValidator<TestModelValidator>();
                    });
                })
                .Configure(x =>
                {
                    x.UseRouting();
                    x.UseEndpoints(builder => builder.MapCarter());
                })
        );
        this.httpClient = this.server.CreateClient();
    }

    [Test]
    [Arguments("POST")]
    [Arguments("PUT")]
    public async Task Should_return_422_on_validation_failure(string httpMethod)
    {
        var res = await this.httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(httpMethod), "/endpointfilter")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new TestModel()), Encoding.UTF8, "application/json")
        });
        await Assert.That( res.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    [Arguments("POST")]
    [Arguments("PUT")]
    public async Task Should_pick_type_to_validate_no_matter_of_delegate_position(string httpMethod)
    {
        var res = await this.httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(httpMethod), "/endpointfilter")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new TestModel()), Encoding.UTF8, "application/json")
        });
        await Assert.That( res.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    [Arguments("POST")]
    [Arguments("PUT")]
    public async Task Should_hit_route_if_validation_successful(string httpMethod)
    {
        var res = await this.httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(httpMethod), "/endpointfilter")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new TestModel { MyStringProperty = "hi", MyIntProperty = 123 }), Encoding.UTF8, "application/json")
        });

        var body = await res.Content.ReadAsStringAsync();

        await Assert.That( body).IsEqualTo(httpMethod);
    }
}
public class NestedRouteExtensionsTests
{
    private readonly TestServer server;

    private readonly HttpClient httpClient;

    public NestedRouteExtensionsTests()
    {
        this.server = new TestServer(
            new WebHostBuilder()
                .ConfigureServices(x =>
                {
                    x.AddLogging(b =>
                    {
                        b.SetMinimumLevel(LogLevel.Debug);
                    });

                    x.AddSingleton<IDependency, Dependency>();

                    x.AddRouting();
                    x.AddCarter(configurator: c => {
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

    [Test]
    [Arguments("GET")]
    public async Task Should_have_nested_class_registered(string httpMethod)
    {
        var res = await this.httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(httpMethod), "/nested")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new TestModel()), Encoding.UTF8, "application/json")
        });
        await Assert.That( res.StatusCode).IsEqualTo(HttpStatusCode.OK);
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
