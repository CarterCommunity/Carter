namespace Carter
{
    using System;
    using FluentValidation;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class CarterDiagnostics
    {
        public static void LogDiscoveredCarterTypes(ICarterBootstrapper bootstrapper, ILogger logger)
        {
            foreach (var validator in bootstrapper.Validators)
            {
                logger.LogDebug("Found validator {ValidatorName}", validator.GetType().Name);
            }

            foreach (var module in bootstrapper.Routes.Values)
            {
                logger.LogDebug("Found module {ModuleName}", module.Module.FullName);
            }

            foreach (var sch in bootstrapper.StatusCodeHandlers)
            {
                logger.LogDebug("Found status code handler {StatusCodeHandlerName}", sch.GetType().FullName);
            }

            foreach (var negotiator in bootstrapper.ResponseNegotiators)
            {
                logger.LogDebug("Found response negotiator {ResponseNegotiatorName}", negotiator.GetType().FullName);
            }
        }
        
        public static void LogDiscoveredCarterTypes(IServiceProvider serviceProvider, ILogger logger)
        {
            foreach (var validator in serviceProvider.GetServices<IValidator>())
            {
                logger.LogDebug("Found validator {ValidatorName}", validator.GetType().Name);
            }

            foreach (var module in serviceProvider.GetServices<CarterModule>())
            {
                logger.LogDebug("Found module {ModuleName}", module.GetType().FullName);
            }

            foreach (var statusCodeHandler in serviceProvider.GetServices<IStatusCodeHandler>())
            {
                logger.LogDebug("Found status code handler {StatusCodeHandlerName}", statusCodeHandler.GetType().FullName);
            }

            foreach (var negotiator in serviceProvider.GetServices<IResponseNegotiator>())
            {
                logger.LogDebug("Found response negotiator {ResponseNegotiatorName}", negotiator.GetType().FullName);
            }
        }
    }
}
