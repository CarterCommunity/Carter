namespace Carter;

using Carter.OpenApi;
using Carter.Response;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class CarterExtensions
{
    /// <summary>
    /// Adds Carter to the specified <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> to configure.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IEndpointRouteBuilder MapCarter(this IEndpointRouteBuilder builder)
    {
        var loggerFactory = builder.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(typeof(CarterConfigurator));

        var carterConfigurator = builder.ServiceProvider.GetRequiredService<CarterConfigurator>();
        carterConfigurator.LogDiscoveredCarterTypes(logger);

        foreach (var carterModuleInterface in builder.ServiceProvider.GetServices<ICarterModule>())
        {
            if (carterModuleInterface is CarterModule carterModule)
            {
                var group = builder.MapGroup(carterModule.basePath);

                if (carterModule.hosts.Any())
                {
                    group = group.RequireHost(carterModule.hosts);
                }

                if (carterModule.requiresAuthorization)
                {
                    group = group.RequireAuthorization(carterModule.authorizationPolicyNames);
                }

                if (!string.IsNullOrWhiteSpace(carterModule.corsPolicyName))
                {
                    group = group.RequireCors(carterModule.corsPolicyName);
                }

                if (carterModule.includeInOpenApi)
                {
                    group.IncludeInOpenApi();
                }

                if (!string.IsNullOrWhiteSpace(carterModule.openApiDescription))
                {
                    group = group.WithDescription(carterModule.openApiDescription);
                }

                if (carterModule.metaData.Any())
                {
                    group = group.WithMetadata(carterModule.metaData);
                }

                if (!string.IsNullOrWhiteSpace(carterModule.openApiName))
                {
                    group = group.WithName(carterModule.openApiName);
                }

                if (!string.IsNullOrWhiteSpace(carterModule.openApisummary))
                {
                    group = group.WithSummary(carterModule.openApisummary);
                }

                if (!string.IsNullOrWhiteSpace(carterModule.openApiDisplayName))
                {
                    group = group.WithDisplayName(carterModule.openApiDisplayName);
                }

                if (!string.IsNullOrWhiteSpace(carterModule.openApiGroupName))
                {
                    group = group.WithGroupName(carterModule.openApiGroupName);
                }

                if (carterModule.tags.Any())
                {
                    group = group.WithTags(carterModule.tags);
                }

                if (!string.IsNullOrWhiteSpace(carterModule.cacheOutputPolicyName))
                {
                    group = group.CacheOutput(carterModule.cacheOutputPolicyName);
                }

                if (carterModule.disableRateLimiting)
                {
                    group = group.DisableRateLimiting();
                }

                if (!string.IsNullOrWhiteSpace(carterModule.rateLimitingPolicyName))
                {
                    group = group.RequireRateLimiting(carterModule.rateLimitingPolicyName);
                }

                if (carterModule.Before != null)
                {
                    group.AddEndpointFilter(async (context, next) =>
                    {
                        var result = carterModule.Before.Invoke(context);
                        if (result != null)
                        {
                            return result;
                        }

                        return await next(context);
                    });
                }

                if (carterModule.After != null)
                {
                    group.AddEndpointFilter(async (context, next) =>
                    {
                        var result = await next(context);
                        carterModule.After.Invoke(context);
                        return result;
                    });
                }

                carterModule.AddRoutes(group);
            }
            else
            {
                carterModuleInterface.AddRoutes(builder);
            }
        }

        return builder;
    }

    /// <summary>
    /// Adds Carter to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add Carter to.</param>
    /// <param name="assemblyCatalog">Optional <see cref="DependencyContextAssemblyCatalog"/> containing assemblies to add to the services collection. If not provided, the default catalog of assemblies is added, which includes Assembly.GetEntryAssembly.</param>
    /// <param name="configurator">Optional <see cref="CarterConfigurator"/> to enable registration of specific types within Carter</param>
    /// <param name="validatorServiceLifetime">Optional <see href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicelifetime"/> to enable registration of validators reporting to ServiceLifetime within Carter. ServiceLifetime defaults to Singleton.</param>
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

        var validatorLocatorLifetime = ServiceLifetime.Singleton;
        foreach (var validator in validators)
        {
            var validatorServiceLifetime = carterConfigurator.ValidatorServiceLifetimeFactory(validator);
            services.Add(
                new ServiceDescriptor(
                    serviceType: typeof(IValidator),
                    implementationType: validator,
                    lifetime: validatorServiceLifetime));

            services.Add(
                new ServiceDescriptor(
                    serviceType: validator,
                    implementationType: validator,
                    lifetime: validatorServiceLifetime));

            if (validatorServiceLifetime > validatorLocatorLifetime)
            {
                validatorLocatorLifetime = validatorServiceLifetime;
            }
        }


        services.Add(
            new ServiceDescriptor(
                serviceType: typeof(IValidatorLocator),
                implementationType: typeof(DefaultValidatorLocator),
                lifetime: validatorLocatorLifetime));

        foreach (var newModule in newModules)
        {
            services.AddSingleton(typeof(ICarterModule), newModule);
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
        if (carterConfigurator.ExcludeResponseNegotiators || carterConfigurator.ResponseNegotiatorTypes.Any())
        {
            responseNegotiators = carterConfigurator.ResponseNegotiatorTypes;
        }
        else
        {
            responseNegotiators = assemblies.SelectMany(x => x.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    typeof(IResponseNegotiator).IsAssignableFrom(t) &&
                    t != typeof(IResponseNegotiator) &&
                    t != typeof(DefaultJsonResponseNegotiator)
                ));

            carterConfigurator.ResponseNegotiatorTypes.AddRange(responseNegotiators);
        }

        return responseNegotiators;
    }

    private static IEnumerable<Type> GetNewModules(CarterConfigurator carterConfigurator,
        IReadOnlyCollection<Assembly> assemblies)
    {
        IEnumerable<Type> modules;
        if (carterConfigurator.ExcludeModules || carterConfigurator.ModuleTypes.Any())
        {
            modules = carterConfigurator.ModuleTypes;
        }
        else
        {
            modules = assemblies.SelectMany(x => x.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    typeof(ICarterModule).IsAssignableFrom(t) &&
                    t != typeof(ICarterModule) &&
                    (
                        t.IsPublic ||                            // public top-level class
                        (t.IsNotPublic && !t.IsNested) ||        // internal top-level class
                        t.IsNestedPublic ||                      // public nested class
                        t.IsNestedAssembly                       // internal nested class
                    )
                ));

            carterConfigurator.ModuleTypes.AddRange(modules);
        }

        return modules;
    }

    private static IEnumerable<Type> GetValidators(CarterConfigurator carterConfigurator,
        IReadOnlyCollection<Assembly> assemblies)
    {
        IEnumerable<Type> validators;

        if (carterConfigurator.ExcludeValidators || carterConfigurator.ValidatorTypes.Any())
        {
            validators = carterConfigurator.ValidatorTypes;
        }
        else
        {
            validators = assemblies.SelectMany(ass => ass.GetTypes())
                .Where(typeof(IValidator).IsAssignableFrom)
                .Where(t => !t.GetTypeInfo().IsAbstract);

            carterConfigurator.ValidatorTypes.AddRange(validators);
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
