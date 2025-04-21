namespace Carter.Tests
{
    using System.Threading.Tasks;
    using Carter.Tests.ContentNegotiation;
    using Carter.Tests.ModelBinding;
    using Carter.Tests.StreamTests;

    public class CarterConfiguratorTests
    {
        [Test]
        public async Task Should_add_single_module()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            configurator.WithModule<TestModule>();

            //Then
            await Assert.That(configurator.ModuleTypes).HasSingleItem();
        }

        [Test]
        public async Task Should_return_same_instance_when_adding_module()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithModule<TestModule>();

            //Then
            await Assert.That(configurator).IsSameReferenceAs(sameconfigurator);
        }

        [Test]
        public async Task Should_add_multiple_modules()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When 
            configurator.WithModules(typeof(TestModule), typeof(StreamModule));

            //Then
            await Assert.That(configurator.ModuleTypes.Count).IsEqualTo(2);
        }

        [Test]
        public async Task Should_return_same_instance_when_adding_multiple_modules()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithModules(typeof(TestModule), typeof(StreamModule));

            //Then
            await Assert.That(configurator).IsSameReferenceAs(sameconfigurator);
        }

        [Test]
        public async Task Should_add_single_validator()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            configurator.WithValidator<TestModelValidator>();

            //Then
            await Assert.That(configurator.ValidatorTypes).HasSingleItem();
        }

        [Test]
        public async Task Should_return_same_instance_when_adding_validator()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithValidator<TestModelValidator>();

            //Then
            await Assert.That(configurator).IsSameReferenceAs(sameconfigurator);
        }

        [Test]
        public async Task Should_add_multiple_validators()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When 
            configurator.WithValidators(typeof(TestModelValidator), typeof(DuplicateTestModelOne));

            //Then
            await Assert.That(configurator.ValidatorTypes.Count).IsEqualTo(2);
        }

        [Test]
        public async Task Should_return_same_instance_when_adding_multiple_validators()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator =
                configurator.WithValidators(typeof(TestModelValidator), typeof(DuplicateTestModelOne));

            //Then
            await Assert.That(configurator).IsSameReferenceAs(sameconfigurator);
        }

        [Test]
        public async Task Should_add_single_responsenegotiator()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            configurator.WithResponseNegotiator<TestResponseNegotiator>();

            //Then
            await Assert.That(configurator.ResponseNegotiatorTypes).HasSingleItem();
        }

        [Test]
        public async Task Should_return_same_instance_when_adding_responsenegotiator()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithResponseNegotiator<TestResponseNegotiator>();

            //Then
            await Assert.That(configurator).IsSameReferenceAs(sameconfigurator);
        }

        [Test]
        public async Task Should_add_multiple_responsenegotiators()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When 
            configurator.WithResponseNegotiators(typeof(TestResponseNegotiator), typeof(TestXmlResponseNegotiator));

            //Then
            await Assert.That(configurator.ResponseNegotiatorTypes.Count).IsEqualTo(2);
        }

        [Test]
        public async Task Should_return_same_instance_when_adding_multiple_responsenegotiators()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator =
                configurator.WithResponseNegotiators(typeof(TestResponseNegotiator), typeof(TestXmlResponseNegotiator));

            //Then
            await Assert.That(configurator).IsSameReferenceAs(sameconfigurator);
        }

        [Test]
        public async Task Should_exclude_modules()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithEmptyModules();

            //Then
            await Assert.That(sameconfigurator.ModuleTypes).IsEmpty();
        }

        [Test]
        public async Task Should_exclude_negotiators()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithResponseNegotiators();

            //Then
            await Assert.That(sameconfigurator.ResponseNegotiatorTypes).IsEmpty();
        }

        [Test]
        public async Task Should_exclude_validators()
        {
            //Given
            var configurator = new CarterConfigurator();

            //When
            var sameconfigurator = configurator.WithEmptyValidators();

            //Then
            await Assert.That(sameconfigurator.ValidatorTypes).IsEmpty();
        }
    }
}
