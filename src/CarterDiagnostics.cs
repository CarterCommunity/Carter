namespace Carter
{
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
    }
}
