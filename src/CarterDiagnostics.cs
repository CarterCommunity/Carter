namespace Carter
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    public class CarterDiagnostics
    {
        private readonly List<Type> modules = new List<Type>();

        private readonly List<Type> responseNegotiators = new List<Type>();

        private readonly List<Type> statusCodeHandlers = new List<Type>();

        private readonly List<Type> validators = new List<Type>();

        public void AddValidator(Type validatorType) => this.validators.Add(validatorType);

        public void AddModule(Type moduleType) => this.modules.Add(moduleType);

        public void AddStatusCodeHandler(Type handlerType) => this.statusCodeHandlers.Add(handlerType);

        public void AddResponseNegotiator(Type responseNegotiatorType) => this.responseNegotiators.Add(responseNegotiatorType);

        public void LogDiscoveredCarterTypes(ILogger logger)
        {
            foreach (var validator in this.validators)
            {
                logger.LogDebug("Found validator {ValidatorName}", validator.Name);
            }

            foreach (var module in this.modules)
            {
                logger.LogDebug("Found module {ModuleName}", module.FullName);
            }

            foreach (var sch in this.statusCodeHandlers)
            {
                logger.LogDebug("Found status code handler {StatusCodeHandlerName}", sch.FullName);
            }

            foreach (var negotiatator in this.responseNegotiators)
            {
                logger.LogDebug("Found response negotiator {ResponseNegotiatorName}", negotiatator.FullName);
            }
        }
    }
}
