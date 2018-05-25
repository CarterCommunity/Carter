namespace Carter.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    internal class CarterDiagnostics
    {
        private readonly List<Type> modules = new List<Type>();

        internal IReadOnlyList<Type> RegisteredModules => this.modules;

        private readonly List<Type> responseNegotiators = new List<Type>();
        
        internal IReadOnlyList<Type> RegisteredResponseNegotiators => this.responseNegotiators;

        private readonly List<Type> statusCodeHandlers = new List<Type>();
        
        internal IReadOnlyList<Type> RegisteredStatusCodeHandlers => this.statusCodeHandlers;

        private readonly List<Type> validators = new List<Type>();
        
        internal IReadOnlyList<Type> RegisteredValidators => this.validators;

        private readonly List<KeyValuePair<Type, string>> routes = new List<KeyValuePair<Type, string>>();

        internal IReadOnlyList<KeyValuePair<Type, string>> RegisteredRoutes => this.routes;
        
        public void AddValidator(Type validatorType) => this.validators.Add(validatorType);

        public void AddModule(Type moduleType) => this.modules.Add(moduleType);

        public void AddStatusCodeHandler(Type handlerType) => this.statusCodeHandlers.Add(handlerType);

        public void AddResponseNegotiator(Type responseNegotiatorType) => this.responseNegotiators.Add(responseNegotiatorType);
        
        public void AddPath(Type moduleType, string path) => this.routes.Add(new KeyValuePair<Type, string>(moduleType, path));

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
