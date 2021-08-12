namespace Carter.Tests
{
    using Carter.Tests.ContentNegotiation;
    using Carter.Tests.ModelBinding;
    using Carter.Tests.StreamTests;
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
            configurator.WithModules(typeof(TestModule), typeof(StreamModule));

            //Then
            Assert.Equal(2, configurator.ModuleTypes.Count);
        }

        [Fact]
        public void Should_return_same_instance_when_adding_multiple_modules()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithModules(typeof(TestModule), typeof(StreamModule));

            //Then
            Assert.Same(configurator, sameconfigurator);
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
            var sameconfigurator =
                configurator.WithValidators(typeof(TestModelValidator), typeof(DuplicateTestModelOne));

            //Then
            Assert.Same(configurator, sameconfigurator);
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
            var sameconfigurator =
                configurator.WithResponseNegotiators(typeof(TestResponseNegotiator), typeof(TestXmlResponseNegotiator));

            //Then
            Assert.Same(configurator, sameconfigurator);
        }

        [Fact]
        public void Should_exclude_modules()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithEmptyModules();
            
            //Then
            Assert.Equal(0, sameconfigurator.ModuleTypes.Count);
        }
        
        [Fact]
        public void Should_exclude_negotiators()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithResponseNegotiators();
            
            //Then
            Assert.Equal(0, sameconfigurator.ResponseNegotiatorTypes.Count);
        }
        
        [Fact]
        public void Should_exclude_validators()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithEmptyValidators();
            
            //Then
            Assert.Equal(0, sameconfigurator.ValidatorTypes.Count);
        }
    }
}
