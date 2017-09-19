namespace Botwin.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using FluentValidation;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Newtonsoft.Json;
    using Xunit;

    public class BindTests
    {
        private readonly TestServer server;
        private readonly HttpClient httpClient;
        
        public BindTests()
        {
            this.server = new TestServer(new WebHostBuilder()
                .ConfigureServices(x => { x.AddBotwin(typeof(TestModule).GetTypeInfo().Assembly); })
                .Configure(x => x.UseBotwin())
            );
            this.httpClient = this.server.CreateClient();
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
            List<ExpandoObject> model = JsonConvert.DeserializeObject<List<ExpandoObject>>(body);
            dynamic first = model.First();
            Assert.Equal(1, model.Count);
            Assert.Equal("No validator found", first.ErrorMessage);
            Assert.Equal("TestModelNoValidator", first.PropertyName);
        }

        [Fact]
        public async Task Should_throw_exception_when_multiple_validators_found()
        {
            var ex = await Record.ExceptionAsync(async () => await this.httpClient.PostAsync("/duplicatevalidator", new StringContent("{\"MyIntProperty\":\"-1\",\"MyStringProperty\":\"\"}", Encoding.UTF8, "application/json")));

            Assert.IsType<InvalidOperationException>(ex);
        }
    }

    public class TestModel
    {
        public int MyIntProperty { get; set; }

        public string MyStringProperty { get; set; }
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

    public class BindModule : BotwinModule
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
        }
    }
}
