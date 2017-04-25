namespace Botwin.Tests
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using FluentValidation;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Xunit;

    public class BindTests
    {

        private readonly TestServer server;
        private readonly HttpClient httpClient;

        public BindTests()
        {
            this.server = new TestServer(new WebHostBuilder()
                                       .ConfigureServices(x =>
                                       {
                                           x.AddSingleton<IAssemblyProvider, TestAssemblyProvider>();
                                           x.AddBotwin();
                                       })
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
        public async Task Should_return_validation_failure_result_when_validator_not_named_properly()
        {
            var res = await this.httpClient.PostAsync("/invalidnamevalidator", new StringContent("{\"MyIntProperty\":\"-1\",\"MyStringProperty\":\"\"}", Encoding.UTF8, "application/json"));
            var body = await res.Content.ReadAsStringAsync();
            List<ExpandoObject> model = JsonConvert.DeserializeObject<List<ExpandoObject>>(body);
            dynamic first = model.First();
            Assert.Equal(1, model.Count);
            Assert.Equal("No validator found", first.ErrorMessage);
            Assert.Equal("TestModelOddValidator", first.PropertyName);
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

    public class TestModelOddValidator
    {
        public int MyIntProperty { get; set; }
        public string MyStringProperty { get; set; }
    }

    public class SnozzCumber : AbstractValidator<TestModelOddValidator>
    {
        public SnozzCumber()
        {
            this.RuleFor(x => x.MyIntProperty).GreaterThan(0);
            this.RuleFor(x => x.MyStringProperty).NotEmpty();
        }
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
                await res.Negotiate(model);
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

            this.Post("/invalidnamevalidator", async (req, res, routeData) =>
            {
                var model = req.BindAndValidate<TestModelOddValidator>();
                if (!model.ValidationResult.IsValid)
                {
                    await res.Negotiate(model.ValidationResult.Errors.Select(x => new { x.PropertyName, x.ErrorMessage }));
                    return;
                }
                await res.Negotiate(model);
            });
        }
    }
}