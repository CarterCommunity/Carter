namespace Carter.Tests;

using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Carter.Tests.ModelBinding;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

public class RouteExtensionsTests(ITestOutputHelper outputHelper)
{
    private HttpClient httpClient;

    private async Task SetupServer()
    {
        var host = new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseTestServer()
                    .ConfigureServices(x =>
                    {
                        x.AddLogging(b =>
                        {
                            b.AddXUnit(outputHelper, y => y.IncludeScopes = true);
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
                    });
            })
            .Build();

        await host.StartAsync();

        this.httpClient = host.GetTestClient();
    }

    [Theory]
    [InlineData("POST")]
    [InlineData("PUT")]
    public async Task Should_return_422_on_validation_failure(string httpMethod)
    {
        await this.SetupServer();
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
        await this.SetupServer();
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
        await this.SetupServer();
        var res = await this.httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(httpMethod), "/endpointfilter")
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(new TestModel { MyStringProperty = "hi", MyIntProperty = 123 }),
                Encoding.UTF8, "application/json")
        });

        var body = await res.Content.ReadAsStringAsync();

        Assert.Equal(httpMethod, body);
    }
    
    [Fact]
    public async Task Should_bind_form_posts()
    {
        await this.SetupServer();
        var formData = new Dictionary<string, string>
        {
            { "MyStringProperty", "hi" },
            { "MyIntProperty", "123" }
        };

        var content = new FormUrlEncodedContent(formData);
        var res = await this.httpClient.PostAsync("/formpost", content);

        var body = await res.Content.ReadAsStringAsync();

        Assert.Contains("{\"myIntProperty\":123,\"myStringProperty\":\"hi\"", body);
    }
}

public class NestedRouteExtensionsTests(ITestOutputHelper outputHelper)
{
    private HttpClient httpClient;

    private async Task SetupServer()
    {
        var host = new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseTestServer()
                    .ConfigureServices(x =>
                    {
                        x.AddLogging(b =>
                        {
                            b.AddXUnit(outputHelper, y => y.IncludeScopes = true);
                            b.SetMinimumLevel(LogLevel.Debug);
                        });
                        x.AddSingleton<IDependency, Dependency>();
                        x.AddRouting();
                        x.AddCarter(configurator: c => { c.WithValidator<TestModelValidator>(); });
                    })
                    .Configure(x =>
                    {
                        x.UseRouting();
                        x.UseEndpoints(builder => builder.MapCarter());
                    });
            })
            .Build();

        await host.StartAsync();

        this.httpClient = host.GetTestClient();
    }

    [Theory]
    [InlineData("GET")]
    public async Task Should_have_nested_class_registered(string httpMethod)
    {
        await this.SetupServer();
        var res = await this.httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(httpMethod), "/nested")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new TestModel()), Encoding.UTF8, "application/json")
        });
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
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
