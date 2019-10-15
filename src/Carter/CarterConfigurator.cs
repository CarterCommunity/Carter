namespace Carter
{
    using System;
    using System.Collections.Generic;
    using FluentValidation;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Configures registrations of certain types within Carter
    /// </summary>
    public class CarterConfigurator
    {
        internal CarterConfigurator()
        {
            this.ModuleTypes = new List<Type>();
            this.ValidatorTypes = new List<Type>();
            this.StatusCodeHandlerTypes = new List<Type>();
            this.ResponseNegotiatorTypes = new List<Type>();
        }

        internal List<Type> ModuleTypes { get; }

        internal List<Type> ValidatorTypes { get; }

        internal List<Type> StatusCodeHandlerTypes { get; }

        internal List<Type> ResponseNegotiatorTypes { get; }
        
        internal void LogDiscoveredCarterTypes(ILogger logger)
        {
            foreach (var validator in this.ValidatorTypes)
            {
                logger.LogDebug("Found validator {ValidatorName}", validator.Name);
            }

            foreach (var module in this.ModuleTypes)
            {
                logger.LogDebug("Found module {ModuleName}", module.FullName);
            }

            foreach (var sch in this.StatusCodeHandlerTypes)
            {
                logger.LogDebug("Found status code handler {StatusCodeHandlerName}", sch.FullName);
            }

            foreach (var negotiator in this.ResponseNegotiatorTypes)
            {
                logger.LogDebug("Found response negotiator {ResponseNegotiatorName}", negotiator.FullName);
            }
        }

        /// <summary>
        /// Register a specific <see cref="CarterModule"/>
        /// </summary>
        /// <typeparam name="TModule">The <see cref="CarterModule"/> to register</typeparam>
        /// <returns><see cref="CarterConfigurator"/></returns>
        public CarterConfigurator WithModule<TModule>() where TModule : CarterModule
        {
            this.ModuleTypes.Add(typeof(TModule));
            return this;
        }

        /// <summary>
        /// Register specific <see cref="CarterModule"/>s
        /// </summary>
        /// <param name="modules">An array of <see cref="CarterModule"/>s</param>
        /// <returns><see cref="CarterConfigurator"/></returns>
        public CarterConfigurator WithModules(params Type[] modules)
        {
            modules.MustDeriveFrom<CarterModule>();
            this.ModuleTypes.AddRange(modules);
            return this;
        }

        /// <summary>
        /// Register a specific <see cref="IValidator"/>
        /// </summary>
        /// <typeparam name="T">The <see cref="IValidator"/> to register</typeparam>
        /// <returns><see cref="CarterConfigurator"/></returns>
        public CarterConfigurator WithValidator<T>() where T : IValidator
        {
            this.ValidatorTypes.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Register specific <see cref="IValidator"/>s
        /// </summary>
        /// <param name="validators">An array of <see cref="IValidator"/>s</param>
        /// <returns><see cref="CarterConfigurator"/></returns>
        public CarterConfigurator WithValidators(params Type[] validators)
        {
            validators.MustDeriveFrom<IValidator>();
            this.ValidatorTypes.AddRange(validators);
            return this;
        }

        /// <summary>
        /// Register a specific <see cref="IStatusCodeHandler"/>
        /// </summary>
        /// <typeparam name="T">The <see cref="IStatusCodeHandler"/> to register</typeparam>
        /// <returns><see cref="CarterConfigurator"/></returns>
        public CarterConfigurator WithStatusCodeHandler<T>() where T : IStatusCodeHandler
        {
            this.StatusCodeHandlerTypes.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Register specific <see cref="IStatusCodeHandler"/>s
        /// </summary>
        /// <param name="statusCodeHandlers">An array of <see cref="IStatusCodeHandler"/>s</param>
        /// <returns><see cref="CarterConfigurator"/></returns>
        public CarterConfigurator WithStatusCodeHandlers(params Type[] statusCodeHandlers)
        {
            statusCodeHandlers.MustDeriveFrom<IStatusCodeHandler>();
            this.StatusCodeHandlerTypes.AddRange(statusCodeHandlers);
            return this;
        }

        /// <summary>
        /// Register a specific <see cref="IResponseNegotiator"/>
        /// </summary>
        /// <typeparam name="T">The <see cref="IResponseNegotiator"/> to register</typeparam>
        /// <returns><see cref="CarterConfigurator"/></returns>
        public CarterConfigurator WithResponseNegotiator<T>() where T : IResponseNegotiator
        {
            this.ResponseNegotiatorTypes.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Register specific <see cref="IStatusCodeHandler"/>s
        /// </summary>
        /// <param name="responseNegotiators">An array of <see cref="IResponseNegotiator"/>s</param>
        /// <returns><see cref="CarterConfigurator"/></returns>
        public CarterConfigurator WithResponseNegotiators(params Type[] responseNegotiators)
        {
            responseNegotiators.MustDeriveFrom<IResponseNegotiator>();
            this.ResponseNegotiatorTypes.AddRange(responseNegotiators);
            return this;
        }
    }
}
