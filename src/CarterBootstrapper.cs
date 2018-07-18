namespace Carter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentValidation;
    using Microsoft.AspNetCore.Http;

    public class CarterBootstrapper : ICarterBootstrapper
    {
        public CarterBootstrapper()
        {
            this.Routes = new Dictionary<(string verb, string path), CarterRoute>();
            this.StatusCodeHandlers = new List<IStatusCodeHandler>();
            this.Validators = new Dictionary<Type, IValidator>();
            this.ResponseNegotiators = new List<IResponseNegotiator>();
        }

        public Dictionary<(string verb, string path), CarterRoute> Routes { get; }
        
        public List<IStatusCodeHandler> StatusCodeHandlers { get; }
        
        public Dictionary<Type, IValidator> Validators { get; }
        
        public List<IResponseNegotiator> ResponseNegotiators { get; }
        
        /// <summary>
        /// A global before handler which is invoked before all routes
        /// </summary>
        public Func<HttpContext, Task<bool>> Before { get; }

        /// <summary>
        /// A global before handler which is invoked after all routes
        /// </summary>
        public Func<HttpContext, Task> After { get; }

        public ICarterBootstrapper RegisterModules(params CarterModule[] modules)
        {
            foreach (var module in modules)
            {
                foreach (var route in module.Routes)
                {
                    this.Routes.Add((route.Key.verb, route.Key.path), route.Value);
                }
            }

            return this;
        }
        
        public ICarterBootstrapper RegisterStatusCodeHandlers(params IStatusCodeHandler[] statusCodeHandlers)
        {
            this.StatusCodeHandlers.AddRange(statusCodeHandlers);
            return this;
        }
       
        public ICarterBootstrapper RegisterValidators(params IValidator[] validators)
        {
            foreach (var validator in validators)
            {
                var validatorType = validator.GetType();
                // ReSharper disable once PossibleNullReferenceException
                var modelType = validatorType.BaseType.GetGenericArguments()[0];
                if (this.Validators.ContainsKey(modelType))
                {
                    throw new InvalidOperationException(
                        $"Ambiguous choice between multiple validators for type {validatorType.Name}. " +
                        $"The validators available are: {String.Join(", ", this.Validators.Select(v => v.GetType().Name))}");
                }
                this.Validators.Add(modelType, validator);
            }
            return this;
        }
        
        public ICarterBootstrapper RegisterResponseNegotiators(params IResponseNegotiator[] negotiators)
        {
            this.ResponseNegotiators.AddRange(negotiators);
            return this;
        }
        
        public IValidator GetValidator<T>() => this.Validators.ContainsKey(typeof(T)) ? this.Validators[typeof(T)] : null;
    }
}