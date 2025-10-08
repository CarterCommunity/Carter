namespace Carter.Tests;

using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

public class ExtensionTests(ITestOutputHelper outputHelper)
{
    private HttpClient httpClient;

    private async Task SetupServer()
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
                            b.AddXUnit(outputHelper, y => y.IncludeScopes = true);
                            b.SetMinimumLevel(LogLevel.Debug);
                        });

                        x.AddSingleton<IDependency, Dependency>();
                        x.AddRouting();
                        x.AddCarter(configurator: c => { c.WithModule<TestModule>(); });
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

    //make a request to /multiquerystring
    //with query string parameters:1,2,3

    [Theory]
    [InlineData("/multiquerystring?id=1&id=2")]
    [InlineData("/multiquerystring?id=1,2")]
    public async Task Should_return_GET_requests_with_multiple_parsed_querystring(string url)
    {
        await this.SetupServer();
        var response = await this.httpClient.GetAsync(url);

        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(200, (int)response.StatusCode);
        Assert.Contains("Managed to parse multiple ints 2", body);
    }

    [Theory]
    [InlineData("/nullablemultiquerystring?id=1&id=2")]
    [InlineData("/nullablemultiquerystring?id=1,2")]
    public async Task Should_return_GET_requests_with_multiple_parsed_querystring_with_nullable_parameters(
        string url)
    {
        await this.SetupServer();
        var response = await this.httpClient.GetAsync(url);

        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(200, (int)response.StatusCode);
        Assert.Contains("Managed to parse multiple Nullable<int>s 2", body);
    }

    [Fact]
    public async Task Should_return_GET_requests_with_parsed_querystring()
    {
        await this.SetupServer();
        const int idToTest = 69;
        var response = await this.httpClient.GetAsync($"/querystring?id={idToTest}");

        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(200, (int)response.StatusCode);
        Assert.Contains($"Managed to parse an int {idToTest}", body);
    }

    [Fact]
    public async Task Should_return_GET_requests_with_parsed_querystring_with_nullable_parameter()
    {
        await this.SetupServer();
        const int idToTest = 69;
        var response = await this.httpClient.GetAsync($"/nullablequerystring?id={idToTest}");

        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(200, (int)response.StatusCode);
        Assert.Contains($"Managed to parse a Nullable<int> {idToTest}", body);
    }

    [Fact]
    public async Task Should_return_POST_request_body_AsStringAsync()
    {
        await this.SetupServer();
        const string content = "Hello";

        var response = await this.httpClient.PostAsync("/asstringasync", new StringContent(content));

        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(200, (int)response.StatusCode);
        Assert.Contains(content, body);
    }

    [Fact]
    public async Task Should_create_file_with_custom_filename()
    {
        await this.SetupServer();
        var multipartFormData = new MultipartFormDataContent();

        using (var ms = new MemoryStream())
        using (var sw = new StreamWriter(ms))
        {
            await sw.WriteLineAsync("Testing");

            multipartFormData.Add(new StreamContent(ms)
            {
                Headers =
                {
                    ContentLength = ms.Length,
                    ContentType = new MediaTypeHeaderValue("text/plain")
                }
            }, "File", "test.txt");

            var res = await this.httpClient.PostAsync("/bindandsavecustomname", multipartFormData);
            var body = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<PathTestModel>(body);

            Assert.True(res.IsSuccessStatusCode);
            Assert.True(Directory.Exists(model.Path));

            var files = Directory.GetFiles(model.Path);

            Assert.NotEmpty(files);
            Assert.True(files.All(x => new FileInfo(x).Name.Equals("mycustom.txt")));

            Directory.Delete(model.Path, true);
        }
    }

    [Fact]
    public async Task Should_create_file_with_default_filename()
    {
        await this.SetupServer();
        var multipartFormData = new MultipartFormDataContent();

        using (var ms = new MemoryStream())
        using (var sw = new StreamWriter(ms))
        {
            await sw.WriteLineAsync("Testing");

            multipartFormData.Add(new StreamContent(ms)
            {
                Headers =
                {
                    ContentLength = ms.Length,
                    ContentType = new MediaTypeHeaderValue("text/plain")
                }
            }, "File", "test.txt");

            var res = await this.httpClient.PostAsync("/bindandsave", multipartFormData);
            var body = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<PathTestModel>(body);

            Assert.True(res.IsSuccessStatusCode);
            Assert.True(Directory.Exists(model.Path));

            var files = Directory.GetFiles(model.Path);

            Assert.NotEmpty(files);
            Assert.True(files.All(x => new FileInfo(x).Name.Equals("test.txt")));

            Directory.Delete(model.Path, true);
        }
    }

    [Fact]
    public async Task Should_return_OK_and_path_for_bindsavefile()
    {
        await this.SetupServer();
        var multipartFormData = new MultipartFormDataContent();

        using (var ms = new MemoryStream())
        using (var sw = new StreamWriter(ms))
        {
            await sw.WriteLineAsync("Testing");

            multipartFormData.Add(new StreamContent(ms)
            {
                Headers =
                {
                    ContentLength = ms.Length,
                    ContentType = new MediaTypeHeaderValue("text/plain")
                }
            }, "File", "test.txt");

            var res = await this.httpClient.PostAsync("/bindandsave", multipartFormData);
            var body = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<PathTestModel>(body);

            Assert.True(res.IsSuccessStatusCode);
            Assert.True(Directory.Exists(model.Path));
            Assert.NotEmpty(Directory.GetFiles(model.Path));

            Directory.Delete(model.Path, true);
        }
    }

    public class PathTestModel
    {
        public string Path { get; set; }
    }
}
