namespace Carter.Tests
{
    using System.Linq;
    using Carter.Tests.ContentNegotiation;
    using Carter.Tests.ModelBinding;
    using Carter.Tests.StreamTests;
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
            var modules = serviceCollection.Where(x => x.ServiceType == typeof(ICarterModule));
            Assert.True(modules.Count() > 1);
        }

        [Fact]
        public void Should_register_modules_passed_in_by_configurator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection, configurator: configurator => configurator.WithModule<TestModule>());

            //Then
            var modules = serviceCollection.Where(x => x.ServiceType == typeof(ICarterModule));
            Assert.Single(modules);
        }
        
        [Fact]
        public void Should_register_multiple_modules_passed_in_by_configurator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection, configurator: configurator => configurator.WithModules(typeof(TestModule), typeof(StreamModule)));

            //Then
            var modules = serviceCollection.Where(x => x.ServiceType == typeof(ICarterModule));
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
        public void Should_register_validators_passed_in_by_configurator()
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
        public void Should_register_multiple_validators_passed_in_by_configurator()
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
        public void Should_register_responsenegotiators_passed_in_by_configurator_and_default_json_negotiator()
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
        public void Should_register_multiple_responsenegotiators_passed_in_by_configurator_and_default_json_negotiator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection, configurator: configurator => configurator.WithResponseNegotiators(typeof(TestResponseNegotiator), typeof(TestXmlResponseNegotiator)));

            //Then
            var responsenegotiators = serviceCollection.Where(x => x.ServiceType == typeof(IResponseNegotiator));
            Assert.Equal(3,responsenegotiators.Count());
        }
        
        [Fact]
        public void Should_register_no_validators_passed_in_by_configurator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection, configurator: configurator => configurator.WithEmptyValidators());

            //Then
            var validators = serviceCollection.Where(x => x.ServiceType == typeof(IValidator));
            Assert.Equal(0,validators.Count());
        }
        
        [Fact]
        public void Should_register_no_modules_passed_in_by_configurator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection, configurator: configurator => configurator.WithEmptyModules());

            //Then
            var modules = serviceCollection.Where(x => x.ServiceType == typeof(ICarterModule));
            Assert.Equal(0,modules.Count());
        }
        
        [Fact]
        public void Should_register_no_response_negotiators_passed_in_by_configurator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            CarterExtensions.AddCarter(serviceCollection, configurator: configurator => configurator.WithEmptyResponseNegotiators());

            //Then
            var responseNegotiators = serviceCollection.Where(x => x.ServiceType == typeof(IResponseNegotiator));
            Assert.Equal(1,responseNegotiators.Count());
        }
    }
}
