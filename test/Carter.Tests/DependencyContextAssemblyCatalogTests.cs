namespace Carter.Tests
{
    using System.Linq;
    using FluentValidation;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class DependencyContextAssemblyCatalogTests
    {
        [Fact]
        public void Should_find_root_assembly_when_walking_assembly_references()
        {
            //Given
            var rootAssembly = typeof(DependencyContextAssemblyCatalogTests).Assembly;

            //When
            var assemblies = DependencyContextAssemblyCatalog.GetReferencedAssemblies(rootAssembly);

            //Then
            Assert.Contains(rootAssembly, assemblies);
        }

        [Fact]
        public void Should_find_referenced_assemblies_that_reference_fluentvalidation_when_walking_assembly_references()
        {
            //Given
            var rootAssembly = typeof(DependencyContextAssemblyCatalogTests).Assembly;

            //When
            var assemblies = DependencyContextAssemblyCatalog.GetReferencedAssemblies(rootAssembly);

            //Then
            Assert.Contains(typeof(ICarterModule).Assembly, assemblies);
        }

        [Fact]
        public void Should_only_find_assemblies_referencing_carter_or_fluentvalidation_when_walking_assembly_references()
        {
            //Given
            var rootAssembly = typeof(DependencyContextAssemblyCatalogTests).Assembly;

            //When
            var assemblies = DependencyContextAssemblyCatalog.GetReferencedAssemblies(rootAssembly);

            //Then
            Assert.DoesNotContain(typeof(IValidator).Assembly, assemblies);
            Assert.All(assemblies, assembly =>
                Assert.Contains(assembly.GetReferencedAssemblies(), reference => reference.Name is "Carter" or "FluentValidation"));
        }

        [Fact]
        public void Should_not_register_open_generic_validators_when_assembly_scanning()
        {
            //Given
            var serviceCollection = new ServiceCollection();

            //When
            serviceCollection.AddCarter();

            //Then
            var validators = serviceCollection.Where(x => x.ServiceType == typeof(IValidator)).ToList();
            Assert.NotEmpty(validators);
            Assert.DoesNotContain(validators, x => x.ImplementationType.ContainsGenericParameters);
        }
    }

    public class OpenGenericValidator<T> : AbstractValidator<T>
    {
    }
}
