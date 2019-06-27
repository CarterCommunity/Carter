namespace Carter.Tests.Modelbinding
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Newtonsoft.Json;
    using Xunit;

    public class BindTests
    {
        public BindTests()
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(x =>
                    {
                        x.AddCarter(configurator: c =>
                            c.WithModule<BindModule>()
                                .WithValidator<TestModelValidator>()
                                .WithValidator<DuplicateTestModelOne>()
                                .WithValidator<DuplicateTestModelTwo>());
                    })
                    .Configure(x => x.UseCarter())
            );
            this.httpClient = server.CreateClient();
        }

        private readonly HttpClient httpClient;

        [Fact]
        public async Task Should_bind_form_data()
        {
            //Given
            var res = await this.httpClient.PostAsync("/bindandvalidate",
                new FormUrlEncodedContent(
                    new[]
                    {
                        new KeyValuePair<string, string>("MyIntProperty", "1"),
                        new KeyValuePair<string, string>("MyStringProperty", "hi there"),
                        new KeyValuePair<string, string>("MyDoubleProperty", "2.3"),
                        new KeyValuePair<string, string>("MyArrayProperty", "1"),
                        new KeyValuePair<string, string>("MyArrayProperty", "2"),
                        new KeyValuePair<string, string>("MyArrayProperty", "3"),
                        new KeyValuePair<string, string>("MyIntArrayProperty", "1"),
                        new KeyValuePair<string, string>("MyIntArrayProperty", "2"),
                        new KeyValuePair<string, string>("MyIntArrayProperty", "3"),
                        new KeyValuePair<string, string>("MyIntListProperty", "1"),
                        new KeyValuePair<string, string>("MyIntListProperty", "2"),
                        new KeyValuePair<string, string>("MyIntListProperty", "3"),
                        new KeyValuePair<string, string>("MyGuidProperty", "E3D2E063-BDC6-426E-A27B-0AAFB5D17AE5")
                    }));

            //When
            var body = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<TestModel>(body);

            //Then
            Assert.Equal(1, model.MyIntProperty);
            Assert.Equal("hi there", model.MyStringProperty);
            Assert.Equal(2.3, model.MyDoubleProperty);
            Assert.Equal(new[] { "1", "2", "3" }, model.MyArrayProperty);
            Assert.Equal(Enumerable.Range(1, 3), model.MyIntArrayProperty);
            Assert.Equal(Enumerable.Range(1, 3), model.MyIntListProperty);
            Assert.Equal(Guid.Parse("E3D2E063-BDC6-426E-A27B-0AAFB5D17AE5"), model.MyGuidProperty);
        }

        [Fact]
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
        public async Task Should_return_instance_of_T()
        {
            var res = await this.httpClient.PostAsync("/bind",
                new StringContent(
                    "{\"MyIntProperty\":\"911\",\"MyStringProperty\":\"Vincent Vega\"}",
                    Encoding.UTF8, "application/json"));
            var body = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<TestModel>(body);

            Assert.Equal(911, model.MyIntProperty);
            Assert.Equal("Vincent Vega", model.MyStringProperty);
        }

        [Fact]
        public async Task Should_return_default_value_of_property_if_property_not_found_on_binding_form()
        {
            var res = await this.httpClient.PostAsync("/bind",
                new FormUrlEncodedContent(
                    new[]
                    {
                        new KeyValuePair<string, string>("\"MyIntProperty\"", "1"),
                        new KeyValuePair<string, string>("MyStringProperty", "Vincent Vega")
                    }));

            var body = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<TestModel>(body);

            Assert.Equal(0, model.MyIntProperty);
            Assert.Equal("Vincent Vega", model.MyStringProperty);
        }

        [Fact]
        public async Task Should_return_instance_of_T_on_successful_validation()
        {
            var res = await this.httpClient.PostAsync("/bindandvalidate",
                new StringContent(
                    "{\"MyIntProperty\":\"911\",\"MyStringProperty\":\"Vincent Vega\"}",
                    Encoding.UTF8, "application/json"));

            var body = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<TestModel>(body);

            Assert.Equal(911, model.MyIntProperty);
            Assert.Equal("Vincent Vega", model.MyStringProperty);
        }

        [Fact]
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

                Assert.True(res.IsSuccessStatusCode);
                Assert.True(Directory.Exists(model.Path));
                Assert.NotEmpty(Directory.GetFiles(model.Path));

                Directory.Delete(model.Path, true);
            }
        }

        [Fact]
        public async Task Should_return_validation_failure_result_when_invalid_data_for_rule()
        {
            var res = await this.httpClient.PostAsync("/bindandvalidate",
                new StringContent("{\"MyIntProperty\":\"-1\",\"MyStringProperty\":\"\"}",
                    Encoding.UTF8, "application/json"));
            var body = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<List<ValidationFailure>>(body);

            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Should_return_validation_failure_result_when_no_validator_found()
        {
            var res = await this.httpClient.PostAsync("/novalidator",
                new StringContent("{\"MyIntProperty\":\"-1\",\"MyStringProperty\":\"\"}",
                    Encoding.UTF8, "application/json"));
            var body = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<List<ExpandoObject>>(body);
            dynamic first = model.First();
            Assert.Single(model);
            Assert.Equal("No validator found", first.errorMessage);
            Assert.Equal("TestModelNoValidator", first.propertyName);
        }

        [Fact]
        public async Task Should_throw_exception_when_multiple_validators_found()
        {
            var ex = await Record.ExceptionAsync(async () =>
                await this.httpClient.PostAsync("/duplicatevalidator",
                    new StringContent("{\"MyIntProperty\":\"-1\",\"MyStringProperty\":\"\"}", Encoding.UTF8,
                        "application/json")));

            Assert.IsType<InvalidOperationException>(ex);
        }
    }
}
