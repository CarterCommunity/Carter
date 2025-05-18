namespace Carter.Tests;

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TUnit.Core.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

public class ExtensionTests
{
    private TestServer server;

    private HttpClient httpClient;

    [Before(Test)]
    public void Setup(TestContext testContext)
    {
        this.server = new TestServer(
            new WebHostBuilder()
                .ConfigureServices(x =>
                {
                    x.AddLogging(b => { b.SetMinimumLevel(LogLevel.Trace);
                        b.AddConsole();
                        b.AddDebug();
                        b.AddProvider(new TUnitLoggerProvider(testContext));
                    });

                    x.AddSingleton<IDependency, Dependency>();

                    x.AddRouting();
                    x.AddCarter(configurator: c =>
                        c.WithModule<TestModule>()
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

    //make a request to /multiquerystring
    //with query string parameters:1,2,3

    [Test]
    [Arguments("/multiquerystring?id=1&id=2")]
    [Arguments("/multiquerystring?id=1,2")]
    public async Task Should_return_GET_requests_with_multiple_parsed_querystring(string url)
    {
        var response = await this.httpClient.GetAsync(url);

        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine("opo");
        var logger = TestContext.Current!.GetDefaultLogger();
        //logger.Log(TUnit.Core.Logging.LogLevel.Trace, "Hekki");
        LoggingExtensions.LogTrace(logger, "Hekki");;
        await Assert.That((int)response.StatusCode).IsEqualTo(200);
        await Assert.That(body).Contains("Managed to parse multiple ints 2");
    }

    [Test]
    [Arguments("/nullablemultiquerystring?id=1&id=2")]
    [Arguments("/nullablemultiquerystring?id=1,2")]
    public async Task Should_return_GET_requests_with_multiple_parsed_querystring_with_nullable_parameters(
        string url)
    {
        var response = await this.httpClient.GetAsync(url);

        var body = await response.Content.ReadAsStringAsync();

        await Assert.That((int)response.StatusCode).IsEqualTo(200);
        await Assert.That(body).Contains("Managed to parse multiple Nullable<int>s 2");
    }

    [Test]
    public async Task Should_return_GET_requests_with_parsed_querystring()
    {
        const int idToTest = 69;
        var response = await this.httpClient.GetAsync($"/querystring?id={idToTest}");

        var body = await response.Content.ReadAsStringAsync();

        await Assert.That((int)response.StatusCode).IsEqualTo(200);
        await Assert.That(body).Contains($"Managed to parse an int {idToTest}");
    }

    [Test]
    public async Task Should_return_GET_requests_with_parsed_querystring_with_nullable_parameter()
    {
        const int idToTest = 69;
        var response = await this.httpClient.GetAsync($"/nullablequerystring?id={idToTest}");

        var body = await response.Content.ReadAsStringAsync();

        await Assert.That((int)response.StatusCode).IsEqualTo(200);
        await Assert.That(body).Contains($"Managed to parse a Nullable<int> {idToTest}");
    }

    [Test]
    public async Task Should_return_POST_request_body_AsStringAsync()
    {
        const string content = "Hello";

        var response = await this.httpClient.PostAsync("/asstringasync", new StringContent(content));

        var body = await response.Content.ReadAsStringAsync();

        await Assert.That((int)response.StatusCode).IsEqualTo(200);
        await Assert.That(body).Contains(content);
    }

    [Test]
    public async Task Should_create_file_with_custom_filename()
    {
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

            await Assert.That(res.IsSuccessStatusCode).IsTrue();
            await Assert.That(Directory.Exists(model.Path)).IsTrue();

            var files = Directory.GetFiles(model.Path);

            await Assert.That(files).IsNotEmpty();
            await Assert.That(files.All(x => new FileInfo(x).Name.Equals("mycustom.txt"))).IsTrue();

            Directory.Delete(model.Path, true);
        }
    }

    [Test]
    public async Task Should_create_file_with_default_filename()
    {
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

            await Assert.That(res.IsSuccessStatusCode).IsTrue();
            await Assert.That(Directory.Exists(model.Path)).IsTrue();

            var files = Directory.GetFiles(model.Path);

            await Assert.That(files).IsNotEmpty();
            ;
            await Assert.That(files.All(x => new FileInfo(x).Name.Equals("test.txt"))).IsTrue();

            Directory.Delete(model.Path, true);
        }
    }

    [Test]
    public async Task Should_return_OK_and_path_for_bindsavefile()
    {
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

            await Assert.That(res.IsSuccessStatusCode).IsTrue();
            await Assert.That(Directory.Exists(model.Path)).IsTrue();
            await Assert.That(Directory.GetFiles(model.Path)).IsNotEmpty();

            Directory.Delete(model.Path, true);
        }
    }

    public class PathTestModel
    {
        public string Path { get; set; }
    }
}
