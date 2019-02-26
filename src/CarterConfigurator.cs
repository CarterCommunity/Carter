namespace Carter
{
    using System;
    using System.Collections.Generic;
    using FluentValidation;

    public class CarterConfigurator
    {
        internal List<Type> moduleTypes = new List<Type>();
        internal List<Type> validatorTypes = new List<Type>();
        internal List<Type> statusCodeHandlerTypes = new List<Type>();
        internal List<Type> responseNegotiatorTypes = new List<Type>();

        public CarterConfigurator WithModule<TModule>() where TModule : CarterModule
        {
            this.moduleTypes.Add(typeof(TModule));
            return this;
        }

        public CarterConfigurator WithModules(params Type[] modules)
        {
            modules.MustDeriveFrom<CarterModule>();
            this.moduleTypes.AddRange(modules);
            return this;
        }

        public CarterConfigurator WithValidator<T>() where T : IValidator
        {
            this.validatorTypes.Add(typeof(T));
            return this;
        }

        public CarterConfigurator WithValidator(params Type[] validators)
        {
            validators.MustDeriveFrom<IValidator>();
            this.validatorTypes.AddRange(validators);
            return this;
        }

        public CarterConfigurator WithStatusCodeHandler<T>() where T : IStatusCodeHandler
        {
            this.statusCodeHandlerTypes.Add(typeof(T));
            return this;
        }

        public CarterConfigurator WithStatusCodeHandlers(params Type[] statusCodeHandlers)
        {
            statusCodeHandlers.MustDeriveFrom<IStatusCodeHandler>();
            return this;
        }

        public CarterConfigurator WithResponseNegotiator<T>() where T : IResponseNegotiator
        {
            this.responseNegotiatorTypes.Add(typeof(T));
            return this;
        }

        public CarterConfigurator WithResponseNegotiators(params Type[] responseNegotiators)
        {
            responseNegotiators.MustDeriveFrom<IResponseNegotiator>();
            this.responseNegotiatorTypes.AddRange(responseNegotiators);
            return this;
        }
        
    }
}