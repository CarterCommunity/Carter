namespace Carter.Tests.ModelBinding.CustomModelBinder
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using global::Newtonsoft.Json;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Xunit;

    public class CustomModelBinderTests
    {
        private readonly HttpClient httpClient;

        public CustomModelBinderTests()
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(x =>
                    {
                        x.AddCarter(configurator: c =>
                        {
                            c.WithModule<CustomModelBinderModule>();
                            c.WithModelBinder<CustomModelBinder>();
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
        public async Task Should_use_custom_modelbinder()
        {
            var request = JsonConvert.SerializeObject(new ModelOnlyNewtonsoftCanParse("hello"), NewtonsoftJsonUtils.JsonSerializerSettings);
            
            var res = await this.httpClient.PostAsync("/bind",new StringContent(request, Encoding.UTF8, "application/json"));

            var json = await res.Content.ReadAsStringAsync();

            var model = JsonConvert.DeserializeObject<ModelOnlyNewtonsoftCanParse>(json, NewtonsoftJsonUtils.JsonSerializerSettings);

            Assert.Equal("world", model.PublicSetterProperty);
            Assert.Equal("hello", model.PrivateSetterProperty);
        }
    }
}
