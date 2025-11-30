namespace Carter.Tests
{
    using System.Linq;
    using Carter.Tests.ContentNegotiation;
    using Carter.Tests.InternalRooms;
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
            serviceCollection.AddCarter();

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
            serviceCollection.AddCarter(configurator: configurator => configurator.WithModule<TestModule>());

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
            serviceCollection.AddCarter(configurator: configurator => configurator.WithModules(typeof(TestModule), typeof(StreamModule)));

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
            serviceCollection.AddCarter();

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
            serviceCollection.AddCarter(configurator: configurator => configurator.WithValidator<TestModelValidator>());

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
            serviceCollection.AddCarter(configurator: configurator => configurator.WithValidators(typeof(TestModelValidator), typeof(DuplicateTestModelOne)));

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
            serviceCollection.AddCarter();

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
            serviceCollection.AddCarter(configurator: configurator => configurator.WithResponseNegotiator<TestResponseNegotiator>());

            //Then
            var responsenegotiators = serviceCollection.Where(x => x.ServiceType == typeof(IResponseNegotiator));
            Assert.Equal(2, responsenegotiators.Count());
        }
        
        [Fact]
        public void Should_register_multiple_response_negotiators_passed_in_by_configurator_and_default_json_negotiator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter(configurator: configurator => configurator.WithResponseNegotiators(typeof(TestResponseNegotiator), typeof(TestXmlResponseNegotiator)));

            //Then
            var responseNegotiators = serviceCollection.Where(x => x.ServiceType == typeof(IResponseNegotiator));
            Assert.Equal(3, responseNegotiators.Count());
        }
        
        [Fact]
        public void Should_register_no_validators_passed_in_by_configurator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter(configurator: configurator => configurator.WithEmptyValidators());

            //Then
            var validators = serviceCollection.Where(x => x.ServiceType == typeof(IValidator));
            Assert.Empty(validators);
        }
        
        [Fact]
        public void Should_register_no_modules_passed_in_by_configurator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter(configurator: configurator => configurator.WithEmptyModules());

            //Then
            var modules = serviceCollection.Where(x => x.ServiceType == typeof(ICarterModule));
            Assert.Empty(modules);
        }
        
        [Fact]
        public void Should_register_no_response_negotiators_passed_in_by_configurator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter(configurator: configurator => configurator.WithEmptyResponseNegotiators());

            //Then
            var responseNegotiators = serviceCollection.Where(x => x.ServiceType == typeof(IResponseNegotiator));
            Assert.Single(responseNegotiators);
        }
        
        [Fact]
        public void Should_register_internal_modules_when_assembly_scanned()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter();

            //Then
            var modules = serviceCollection.Where(x => x.ServiceType == typeof(ICarterModule));
            var internalModule = modules.FirstOrDefault(x => x.ImplementationType == typeof(InternalRoomModule));
            Assert.NotNull(internalModule);
        }

        [Fact]
        public void Should_register_internal_module_passed_in_by_configurator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter(configurator: configurator => configurator.WithModule<InternalRoomModule>());

            //Then
            var modules = serviceCollection.Where(x => x.ServiceType == typeof(ICarterModule));
            var internalModule = modules.FirstOrDefault(x => x.ImplementationType == typeof(InternalRoomModule));
            Assert.NotNull(internalModule);
        }

        [Fact]
        public void Should_register_multiple_internal_modules_passed_in_by_configurator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter(configurator: configurator => 
                configurator.WithModules(typeof(InternalRoomModule), typeof(NestedInternalRoomModuleWrapper.NestedInternalRoomModule)));

            //Then
            var modules = serviceCollection.Where(x => x.ServiceType == typeof(ICarterModule)).ToList();
            Assert.Contains(modules, m => m.ImplementationType == typeof(InternalRoomModule));
            Assert.Contains(modules, m => m.ImplementationType == typeof(NestedInternalRoomModuleWrapper.NestedInternalRoomModule));
            Assert.Equal(2, modules.Count());
        }

        [Fact]
        public void Should_register_internal_validators_when_assembly_scanned()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter();

            //Then
            var validators = serviceCollection.Where(x => x.ServiceType == typeof(IValidator));
            var internalValidator = validators.FirstOrDefault(x => x.ImplementationType == typeof(InternalRoomModelValidator));
            Assert.NotNull(internalValidator);
        }

        [Fact]
        public void Should_register_internal_validator_passed_in_by_configurator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter(configurator: configurator => configurator.WithValidator<InternalRoomModelValidator>());

            //Then
            var validators = serviceCollection.Where(x => x.ServiceType == typeof(IValidator)).ToList();
            var internalValidator = validators.FirstOrDefault(x => x.ImplementationType == typeof(InternalRoomModelValidator));
            Assert.NotNull(internalValidator);
            Assert.Single(validators);
        }

        [Fact]
        public void Should_register_multiple_internal_validators_passed_in_by_configurator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter(configurator: configurator => 
                configurator.WithValidators(typeof(InternalRoomModelValidator), typeof(NestedInternalTestModelValidatorWrapper.InternalTestModelValidator)));

            //Then
            var validators = serviceCollection.Where(x => x.ServiceType == typeof(IValidator)).ToList();
            Assert.Contains(validators, v => v.ImplementationType == typeof(InternalRoomModelValidator));
            Assert.Contains(validators, v => v.ImplementationType == typeof(NestedInternalTestModelValidatorWrapper.InternalTestModelValidator));
            Assert.Equal(2, validators.Count());
        }

        [Fact]
        public void Should_register_internal_response_negotiators_when_assembly_scanned()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter();

            //Then
            var responseNegotiators = serviceCollection.Where(x => x.ServiceType == typeof(IResponseNegotiator));
            var internalNegotiator = responseNegotiators.FirstOrDefault(x => x.ImplementationType == typeof(InternalResponseNegotiator));
            Assert.NotNull(internalNegotiator);
        }

        [Fact]
        public void Should_register_internal_response_negotiator_passed_in_by_configurator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter(configurator: configurator => configurator.WithResponseNegotiator<InternalResponseNegotiator>());

            //Then
            var responseNegotiators = serviceCollection.Where(x => x.ServiceType == typeof(IResponseNegotiator));
            var internalNegotiator = responseNegotiators.FirstOrDefault(x => x.ImplementationType == typeof(InternalResponseNegotiator));
            Assert.NotNull(internalNegotiator);
        }

        [Fact]
        public void Should_register_multiple_internal_response_negotiators_passed_in_by_configurator()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter(configurator: configurator => 
                configurator.WithResponseNegotiators(typeof(InternalResponseNegotiator), typeof(NestedInternalResponseNegotiatorWrapper.NestedInternalResponseNegotiator)));

            //Then
            var responseNegotiators = serviceCollection.Where(x => x.ServiceType == typeof(IResponseNegotiator)).ToList();
            Assert.Contains(responseNegotiators, r => r.ImplementationType == typeof(InternalResponseNegotiator));
            Assert.Contains(responseNegotiators, r => r.ImplementationType == typeof(NestedInternalResponseNegotiatorWrapper.NestedInternalResponseNegotiator));
        }

        [Fact]
        public void Should_register_mix_of_public_and_internal_modules()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter(configurator: configurator => 
                configurator.WithModules(typeof(TestModule), typeof(InternalRoomModule)));

            //Then
            var modules = serviceCollection.Where(x => x.ServiceType == typeof(ICarterModule)).ToList();
            Assert.Contains(modules, m => m.ImplementationType == typeof(TestModule));
            Assert.Contains(modules, m => m.ImplementationType == typeof(InternalRoomModule));
            Assert.Equal(2, modules.Count());
        }

        [Fact]
        public void Should_register_mix_of_public_and_internal_validators()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter(configurator: configurator => 
                configurator.WithValidators(typeof(TestModelValidator), typeof(InternalRoomModelValidator)));

            //Then
            var validators = serviceCollection.Where(x => x.ServiceType == typeof(IValidator)).ToList();
            Assert.Contains(validators, v => v.ImplementationType == typeof(TestModelValidator));
            Assert.Contains(validators, v => v.ImplementationType == typeof(InternalRoomModelValidator));
            Assert.Equal(2, validators.Count());
        }
    }
}
