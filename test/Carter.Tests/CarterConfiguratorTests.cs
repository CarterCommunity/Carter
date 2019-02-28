namespace Carter.Tests
{
    using System;
    using Xunit;

    public class CarterConfiguratorTests
    {
        [Fact]
        public void Should_add_single_module()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            configurator.WithModule<TestModule>();

            //Then
            Assert.Single(configurator.ModuleTypes);
        }

        [Fact]
        public void Should_return_same_instance_when_adding_module()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithModule<TestModule>();

            //Then
            Assert.Same(configurator, sameconfigurator);
        }

        [Fact]
        public void Should_add_multiple_modules()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When 
            configurator.WithModules(typeof(TestModule), typeof(BindModule));

            //Then
            Assert.Equal(2, configurator.ModuleTypes.Count);
        }

        [Fact]
        public void Should_return_same_instance_when_adding_multiple_modules()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithModules(typeof(TestModule), typeof(BindModule));

            //Then
            Assert.Same(configurator, sameconfigurator);
        }

        [Fact]
        public void Should_throw_when_registering_multiple_modules_with_invalid_types()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When & Then
            var ex = Assert.Throws<ArgumentException>(() => configurator.WithModules(typeof(int), typeof(string)));

            Assert.Equal("Types must derive from CarterModule, failing types: System.Int32,System.String", ex.Message);
        }

        [Fact]
        public void Should_add_single_validator()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            configurator.WithValidator<TestModelValidator>();

            //Then
            Assert.Single(configurator.ValidatorTypes);
        }

        [Fact]
        public void Should_return_same_instance_when_adding_validator()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithValidator<TestModelValidator>();

            //Then
            Assert.Same(configurator, sameconfigurator);
        }
        
        [Fact]
        public void Should_add_multiple_validators()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When 
            configurator.WithValidators(typeof(TestModelValidator), typeof(DuplicateTestModelOne));

            //Then
            Assert.Equal(2, configurator.ValidatorTypes.Count);
        }

        [Fact]
        public void Should_return_same_instance_when_adding_multiple_validators()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithValidators(typeof(TestModelValidator), typeof(DuplicateTestModelOne));

            //Then
            Assert.Same(configurator, sameconfigurator);
        }

        [Fact]
        public void Should_throw_when_registering_multiple_validators_with_invalid_types()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When & Then
            var ex = Assert.Throws<ArgumentException>(() => configurator.WithValidators(typeof(int), typeof(string)));

            Assert.Equal("Types must derive from IValidator, failing types: System.Int32,System.String", ex.Message);
        }
        
        [Fact]
        public void Should_add_single_statuscodehandler()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            configurator.WithStatusCodeHandler<TeapotStatusCodeHandler>();

            //Then
            Assert.Single(configurator.StatusCodeHandlerTypes);
        }

        [Fact]
        public void Should_return_same_instance_when_adding_statuscodehandler()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithStatusCodeHandler<TeapotStatusCodeHandler>();

            //Then
            Assert.Same(configurator, sameconfigurator);
        }
        
        [Fact]
        public void Should_add_multiple_statuscodehandlers()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When 
            configurator.WithStatusCodeHandlers(typeof(TeapotStatusCodeHandler), typeof(NoOpStatusCodeHandler));

            //Then
            Assert.Equal(2, configurator.StatusCodeHandlerTypes.Count);
        }

        [Fact]
        public void Should_return_same_instance_when_adding_multiple_statuscodehandlers()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithStatusCodeHandlers(typeof(TeapotStatusCodeHandler), typeof(NoOpStatusCodeHandler));

            //Then
            Assert.Same(configurator, sameconfigurator);
        }

        [Fact]
        public void Should_throw_when_registering_multiple_statuscodehandlers_with_invalid_types()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When & Then
            var ex = Assert.Throws<ArgumentException>(() => configurator.WithStatusCodeHandlers(typeof(int), typeof(string)));

            Assert.Equal("Types must derive from IStatusCodeHandler, failing types: System.Int32,System.String", ex.Message);
        }
        
        [Fact]
        public void Should_add_single_responsenegotiator()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            configurator.WithResponseNegotiator<TestResponseNegotiator>();

            //Then
            Assert.Single(configurator.ResponseNegotiatorTypes);
        }

        [Fact]
        public void Should_return_same_instance_when_adding_responsenegotiator()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithResponseNegotiator<TestResponseNegotiator>();

            //Then
            Assert.Same(configurator, sameconfigurator);
        }
        
        [Fact]
        public void Should_add_multiple_responsenegotiators()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When 
            configurator.WithResponseNegotiators(typeof(TestResponseNegotiator), typeof(TestXmlResponseNegotiator));

            //Then
            Assert.Equal(2, configurator.ResponseNegotiatorTypes.Count);
        }

        [Fact]
        public void Should_return_same_instance_when_adding_multiple_responsenegotiators()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithResponseNegotiators(typeof(TestResponseNegotiator), typeof(TestXmlResponseNegotiator));

            //Then
            Assert.Same(configurator, sameconfigurator);
        }

        [Fact]
        public void Should_throw_when_registering_multiple_responsenegotiators_with_invalid_types()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When & Then
            var ex = Assert.Throws<ArgumentException>(() => configurator.WithResponseNegotiators(typeof(int), typeof(string)));

            Assert.Equal("Types must derive from IResponseNegotiator, failing types: System.Int32,System.String", ex.Message);
        }

    }
}
