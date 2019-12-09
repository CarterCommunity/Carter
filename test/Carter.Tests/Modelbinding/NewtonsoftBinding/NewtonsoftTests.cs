namespace Carter.Tests.ModelBinding.NewtonsoftBinding
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Carter.ModelBinding;
    using Carter.Tests.Modelbinding;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Xunit;

    public class NewtonsoftTests
    {
        private readonly HttpClient httpClient;

        public NewtonsoftTests()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                ContractResolver = new NewtonsoftJsonUtils.PrivateSetterCamelCasePropertyNamesContractResolver(),
                Converters = new JsonConverter[] {new StringEnumConverter()},
            };
            
            var server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(x =>
                    {
                        x.AddCarter(configurator: c =>
                            c.WithModule<NewtonsoftModule>());

                        x.AddSingleton<IModelBinder, NewtonsoftJsonModelBinder>();
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
        public async Task Model_with_private_getter_should_parse()
        {
            var request = JsonConvert.SerializeObject(new ModelOnlyNewtonsoftCanParse("hello"));
            var res = await this.httpClient.PostAsync("/bind", 
                 new StringContent(request));

            Assert.True(res.IsSuccessStatusCode);
            var json = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<ModelOnlyNewtonsoftCanParse>(json, NewtonsoftJsonUtils.JsonSerializerSettings);

            Assert.Equal("world", model.PublicSetterProperty);
            Assert.Equal("hello", model.PrivateSetterProperty);
        }
    }
}
