namespace Carter.Tests
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
    using Carter.ModelBinding;
    using Carter.Response;
    using FluentValidation;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Newtonsoft.Json;
    using Xunit;

    public class BindTests
    {
        public BindTests()
        {
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(x => { x.AddCarter(); })
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
                        new KeyValuePair<string, string>("MyIntListProperty", "3")
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
            var res = await this.httpClient.PostAsync("/bind", new StringContent("{\"MyIntProperty\":\"911\",\"MyStringProperty\":\"Vincent Vega\"}", Encoding.UTF8, "application/json"));
            var body = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<TestModel>(body);

            Assert.Equal(911, model.MyIntProperty);
            Assert.Equal("Vincent Vega", model.MyStringProperty);
        }

        [Fact]
        public async Task Should_return_instance_of_T_on_successful_validation()
        {
            var res = await this.httpClient.PostAsync("/bindandvalidate", new StringContent("{\"MyIntProperty\":\"911\",\"MyStringProperty\":\"Vincent Vega\"}", Encoding.UTF8, "application/json"));

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
            var res = await this.httpClient.PostAsync("/bindandvalidate", new StringContent("{\"MyIntProperty\":\"-1\",\"MyStringProperty\":\"\"}", Encoding.UTF8, "application/json"));
            var body = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<List<ValidationFailure>>(body);

            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Should_return_validation_failure_result_when_no_validator_found()
        {
            var res = await this.httpClient.PostAsync("/novalidator", new StringContent("{\"MyIntProperty\":\"-1\",\"MyStringProperty\":\"\"}", Encoding.UTF8, "application/json"));
            var body = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<List<ExpandoObject>>(body);
            dynamic first = model.First();
            Assert.Equal(1, model.Count);
            Assert.Equal("No validator found", first.errorMessage);
            Assert.Equal("TestModelNoValidator", first.propertyName);
        }

        [Fact]
        public async Task Should_throw_exception_when_multiple_validators_found()
        {
            var ex = await Record.ExceptionAsync(async () =>
                await this.httpClient.PostAsync("/duplicatevalidator", new StringContent("{\"MyIntProperty\":\"-1\",\"MyStringProperty\":\"\"}", Encoding.UTF8, "application/json")));

            Assert.IsType<InvalidOperationException>(ex);
        }
    }

    public class TestModel
    {
        public int MyIntProperty { get; set; }

        public string MyStringProperty { get; set; }

        public double MyDoubleProperty { get; set; }

        public string[] MyArrayProperty { get; set; }

        public IEnumerable<int> MyIntArrayProperty { get; set; }

        public IEnumerable<int> MyIntListProperty { get; set; }
    }

    public class TestModelValidator : AbstractValidator<TestModel>
    {
        public TestModelValidator()
        {
            this.RuleFor(x => x.MyIntProperty).GreaterThan(0);
            this.RuleFor(x => x.MyStringProperty).NotEmpty();
        }
    }

    public class TestModelNoValidator
    {
        public int MyIntProperty { get; set; }

        public string MyStringProperty { get; set; }
    }

    public class DuplicateTestModel
    {
    }

    public class DuplicateTestModelOne : AbstractValidator<DuplicateTestModel>
    {
    }

    public class DuplicateTestModelTwo : AbstractValidator<DuplicateTestModel>
    {
    }

    public class PathTestModel
    {
        public string Path { get; set; }
    }

    public class BindModule : CarterModule
    {
        public BindModule()
        {
            this.Post("/bind", async (req, res, routeData) =>
            {
                var model = req.Bind<TestModel>();
                await res.Negotiate(model);
            });

            this.Post("/bindandvalidate", async (req, res, routeData) =>
            {
                var model = req.BindAndValidate<TestModel>();
                if (!model.ValidationResult.IsValid)
                {
                    await res.Negotiate(model.ValidationResult.Errors);
                    return;
                }

                await res.Negotiate(model.Data);
            });

            this.Post("/novalidator", async (req, res, routeData) =>
            {
                var model = req.BindAndValidate<TestModelNoValidator>();
                if (!model.ValidationResult.IsValid)
                {
                    await res.Negotiate(model.ValidationResult.Errors.Select(x => new { x.PropertyName, x.ErrorMessage }));
                    return;
                }

                await res.Negotiate(model);
            });

            this.Post("/duplicatevalidator", async (req, res, routeData) =>
            {
                var model = req.BindAndValidate<DuplicateTestModel>();
                if (!model.ValidationResult.IsValid)
                {
                    await res.Negotiate(model.ValidationResult.Errors.Select(x => new { x.PropertyName, x.ErrorMessage }));
                    return;
                }

                await res.Negotiate(model.Data);
            });

            this.Post("/bindandsave", async (req, res, routeData) =>
            {
                var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

                await req.BindAndSaveFile(filePath);

                await res.Negotiate(new PathTestModel { Path = filePath });
            });

            this.Post("/bindandsavecustomname", async (req, res, routeData) =>
            {
                var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

                await req.BindAndSaveFile(filePath, "mycustom.txt");

                await res.Negotiate(new PathTestModel { Path = filePath });
            });
        }
    }
}
