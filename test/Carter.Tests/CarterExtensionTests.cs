namespace Carter.Tests
{
    using System.Linq;
    using Carter.Tests.ContentNegotiation;
    using Carter.Tests.Modelbinding;
    using Carter.Tests.StatusCodeHandlers;
    using FluentValidation;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class CarterExtensionTests
    {
        [Fact]
        public void Should_register_assembly_scanned_modules_when_no_configurator_used()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection);

            //Then
            var modules = serviceCollection.Where(x => x.ServiceType == typeof(CarterModule));
            Assert.True(modules.Count() > 1);
        }

        [Fact]
        public void Should_register_modules_passed_in_by_configutator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection, configurator: configurator => configurator.WithModule<TestModule>());

            //Then
            var modules = serviceCollection.Where(x => x.ServiceType == typeof(CarterModule));
            Assert.Single(modules);
        }
        
        [Fact]
        public void Should_register_multiple_modules_passed_in_by_configutator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection, configurator: configurator => configurator.WithModules(typeof(TestModule), typeof(BindModule)));

            //Then
            var modules = serviceCollection.Where(x => x.ServiceType == typeof(CarterModule));
            Assert.Equal(2,modules.Count());
        }
        
        [Fact]
        public void Should_register_assembly_scanned_valdators_when_no_configurator_used()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection);

            //Then
            var validators = serviceCollection.Where(x => x.ServiceType == typeof(IValidator));
            Assert.True(validators.Count() > 1);
        }

        [Fact]
        public void Should_register_validators_passed_in_by_configutator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection, configurator: configurator => configurator.WithValidator<TestModelValidator>());

            //Then
            var validators = serviceCollection.Where(x => x.ServiceType == typeof(IValidator));
            Assert.Single(validators);
        }
        
        [Fact]
        public void Should_register_multiple_validators_passed_in_by_configutator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection, configurator: configurator => configurator.WithValidators(typeof(TestModelValidator), typeof(DuplicateTestModelOne)));

            //Then
            var validators = serviceCollection.Where(x => x.ServiceType == typeof(IValidator));
            Assert.Equal(2,validators.Count());
        }
        
        [Fact]
        public void Should_register_assembly_scanned_statuscodehandlers_when_no_configurator_used()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection);

            //Then
            var statuscodehandlers = serviceCollection.Where(x => x.ServiceType == typeof(IStatusCodeHandler));
            Assert.True(statuscodehandlers.Count() > 1);
        }

        [Fact]
        public void Should_register_statuscodehandlers_passed_in_by_configutator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection, configurator: configurator => configurator.WithStatusCodeHandler<TeapotStatusCodeHandler>());

            //Then
            var statuscodehandlers = serviceCollection.Where(x => x.ServiceType == typeof(IStatusCodeHandler));
            Assert.Single(statuscodehandlers);
        }
        
        [Fact]
        public void Should_register_multiple_statuscodehandlers_passed_in_by_configutator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection, configurator: configurator => configurator.WithStatusCodeHandlers(typeof(TeapotStatusCodeHandler), typeof(NoOpStatusCodeHandler)));

            //Then
            var statuscodehandlers = serviceCollection.Where(x => x.ServiceType == typeof(IStatusCodeHandler));
            Assert.Equal(2,statuscodehandlers.Count());
        }
        
        [Fact]
        public void Should_register_assembly_scanned_responsenegotiators_when_no_configurator_used()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection);

            //Then
            var responsenegotiators = serviceCollection.Where(x => x.ServiceType == typeof(IResponseNegotiator));
            Assert.True(responsenegotiators.Count() > 1);
        }

        [Fact]
        public void Should_register_responsenegotiators_passed_in_by_configutator_and_default_json_negotiator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection, configurator: configurator => configurator.WithResponseNegotiator<TestResponseNegotiator>());

            //Then
            var responsenegotiators = serviceCollection.Where(x => x.ServiceType == typeof(IResponseNegotiator));
            Assert.Equal(2, responsenegotiators.Count());
        }
        
        [Fact]
        public void Should_register_multiple_responsenegotiators_passed_in_by_configutator_and_default_json_negotiator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection, configurator: configurator => configurator.WithResponseNegotiators(typeof(TestResponseNegotiator), typeof(TestXmlResponseNegotiator)));

            //Then
            var responsenegotiators = serviceCollection.Where(x => x.ServiceType == typeof(IResponseNegotiator));
            Assert.Equal(3,responsenegotiators.Count());
        }
    }
}
