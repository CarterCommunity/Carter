namespace Carter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Carter.Response;
    using FluentValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class CarterExtensions
    {
        /// <summary>
        /// Adds Carter to the specified <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to configure.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static void MapCarter(this IEndpointRouteBuilder builder)
        {
            var loggerFactory = builder.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(typeof(CarterConfigurator));
            
            var carterConfigurator = builder.ServiceProvider.GetRequiredService<CarterConfigurator>();
            carterConfigurator.LogDiscoveredCarterTypes(logger);

            foreach (var newCarterModule in builder.ServiceProvider.GetServices<ICarterModule>())
            {
                newCarterModule.AddRoutes(builder);
            }
        }

        /// <summary>
        /// Adds Carter to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add Carter to.</param>
        /// <param name="assemblyCatalog">Optional <see cref="DependencyContextAssemblyCatalog"/> containing assemblies to add to the services collection. If not provided, the default catalog of assemblies is added, which includes Assembly.GetEntryAssembly.</param>
        /// <param name="configurator">Optional <see cref="CarterConfigurator"/> to enable registration of specific types within Carter</param>
        public static IServiceCollection AddCarter(this IServiceCollection services,
            DependencyContextAssemblyCatalog assemblyCatalog = null,
            Action<CarterConfigurator> configurator = null)
        {
            assemblyCatalog ??= new DependencyContextAssemblyCatalog();

            var config = new CarterConfigurator();
            configurator?.Invoke(config);

            WireupCarter(services, assemblyCatalog, config);

            return services;
        }

        private static void WireupCarter(this IServiceCollection services,
            DependencyContextAssemblyCatalog assemblyCatalog, CarterConfigurator carterConfigurator)
        {
            var assemblies = assemblyCatalog.GetAssemblies();

            var validators = GetValidators(carterConfigurator, assemblies);

            var newModules = GetNewModules(carterConfigurator, assemblies);

            var responseNegotiators = GetResponseNegotiators(carterConfigurator, assemblies);

            services.AddSingleton(carterConfigurator);

            foreach (var validator in validators)
            {
                services.AddSingleton(typeof(IValidator), validator);
                services.AddSingleton(validator);
            }

            services.AddSingleton<IValidatorLocator, DefaultValidatorLocator>();

            //services.AddRouting();

            foreach (var newModule in newModules)
            {
                //services.AddScoped(newModule);
                services.AddScoped(typeof(ICarterModule), newModule);
            }

            foreach (var negotiator in responseNegotiators)
            {
                services.AddSingleton(typeof(IResponseNegotiator), negotiator);
            }

            services.AddSingleton<IResponseNegotiator, DefaultJsonResponseNegotiator>();
        }

        private static IEnumerable<Type> GetResponseNegotiators(CarterConfigurator carterConfigurator,
            IReadOnlyCollection<Assembly> assemblies)
        {
            IEnumerable<Type> responseNegotiators;
            if (!carterConfigurator.ResponseNegotiatorTypes.Any())
            {
                responseNegotiators = assemblies.SelectMany(x => x.GetTypes()
                    .Where(t =>
                        !t.IsAbstract &&
                        typeof(IResponseNegotiator).IsAssignableFrom(t) &&
                        t != typeof(IResponseNegotiator) &&
                        t != typeof(DefaultJsonResponseNegotiator) &&
                        t != typeof(NewtonsoftJsonResponseNegotiator)
                    ));

                carterConfigurator.ResponseNegotiatorTypes.AddRange(responseNegotiators);
            }
            else
            {
                responseNegotiators = carterConfigurator.ResponseNegotiatorTypes;
            }

            return responseNegotiators;
        }
        
        private static IEnumerable<Type> GetNewModules(CarterConfigurator carterConfigurator, IReadOnlyCollection<Assembly> assemblies)
        {
            IEnumerable<Type> modules;
            if (!carterConfigurator.ModuleTypes.Any())
            {
                modules = assemblies.SelectMany(x => x.GetTypes()
                    .Where(t =>
                        !t.IsAbstract &&
                        typeof(ICarterModule).IsAssignableFrom(t) &&
                        t != typeof(ICarterModule) &&
                        t.IsPublic
                    ));

                carterConfigurator.ModuleTypes.AddRange(modules);
            }
            else
            {
                modules = carterConfigurator.ModuleTypes;
            }

            return modules;
        }

        private static IEnumerable<Type> GetValidators(CarterConfigurator carterConfigurator,
            IReadOnlyCollection<Assembly> assemblies)
        {
            IEnumerable<Type> validators;
            if (!carterConfigurator.ValidatorTypes.Any())
            {
                validators = assemblies.SelectMany(ass => ass.GetTypes())
                    .Where(typeof(IValidator).IsAssignableFrom)
                    .Where(t => !t.GetTypeInfo().IsAbstract);

                carterConfigurator.ValidatorTypes.AddRange(validators);
            }
            else
            {
                validators = carterConfigurator.ValidatorTypes;
            }

            return validators;
        }

        private class CompositeConventionBuilder : IEndpointConventionBuilder
        {
            private readonly List<IEndpointConventionBuilder> _builders;

            public CompositeConventionBuilder(List<IEndpointConventionBuilder> builders)
            {
                _builders = builders;
            }

            public void Add(Action<EndpointBuilder> convention)
            {
                foreach (var builder in _builders)
                {
                    builder.Add(convention);
                }
            }
        }
    }
}
