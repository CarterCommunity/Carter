namespace Carter.Tests.ModelBinding.NewtonsoftBinding
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Carter.ModelBinding;
    using Carter.Tests.Modelbinding;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Newtonsoft.Json;
    using Xunit;

    public class NewtonsoftBinderTests
    {
        private readonly HttpClient httpClient;

        public NewtonsoftBinderTests()
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(x =>
                    {
                        x.AddCarter(configurator: c =>
                        {
                            c.WithModule<NewtonsoftBinderModule>();
                            c.WithModelBinder<NewtonsoftJsonModelBinder>();
                        });
                    })
                    .Configure(x =>
                    {
                        x.UseRouting();
                        x.UseEndpoints(builder => builder.MapCarter());
                    })
            );
            this.httpClient = server.CreateClient();
        }

        [Fact]
        public async Task Should_use_newtonsoft_modelbinder()
        {
            var res = await this.httpClient.PostAsync("/bind",
                new StringContent(
                    "{\"MyIntProperty\":911,\"MyStringProperty\":\"Vincent Vega\"}",
                    Encoding.UTF8, "application/json"));

            var body = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<TestModel>(body);

            Assert.Equal(911, model.MyIntProperty);
            Assert.Equal("Vincent Vega", model.MyStringProperty);
        }
    }
}
