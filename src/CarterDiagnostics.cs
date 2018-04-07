namespace Carter
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    public class CarterDiagnostics
    {
        private List<Type> validators = new List<Type>();
        private List<Type> modules = new List<Type>();
        private List<Type> statusCodeHandlers = new List<Type>();
        private List<Type> responseNegotiators = new List<Type>();

        public void AddValidator(Type validatorType)
            => validators.Add(validatorType);

        public void AddModule(Type moduleType)
            => modules.Add(moduleType);

        public void AddStatusCodeHandler(Type handlerType)
            => statusCodeHandlers.Add(handlerType);

        public void AddResponseNegotiator(Type responseNegotiatorType)
            => responseNegotiators.Add(responseNegotiatorType);

        public void LogDiscoveredModules(ILogger logger)
        {
            foreach (var validator in validators)
            {
                logger.LogTrace("Found validator {ValidatorName}", validator.Name);
            }

            foreach (var module in modules)
            {
                logger.LogTrace("Found module {ModuleName}", module.FullName);
            }

            foreach (var sch in statusCodeHandlers)
            {
                logger.LogTrace("Found status code handler {StatusCodeHandlerName}", sch.FullName);
            }

            foreach (var negotiatator in responseNegotiators)
            {
                logger.LogTrace("Found response negotiator {ResponseNegotiatorName}", negotiatator.FullName);
            }
        }
    }
}
