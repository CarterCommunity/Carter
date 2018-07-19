namespace Carter
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using FluentValidation;
    using Microsoft.AspNetCore.Http;
    
    public class CarterBootstrapper : ICarterBootstrapper
    {
        private readonly Dictionary<(string verb, string path), CarterRoute> routes;
        private readonly List<IStatusCodeHandler> statusCodeHandlers;
        private readonly List<IValidator> validators;
        private readonly List<IResponseNegotiator> responseNegotiators;

        public CarterBootstrapper()
        {
            this.routes = new Dictionary<(string verb, string path), CarterRoute>();
            this.statusCodeHandlers = new List<IStatusCodeHandler>();
            this.validators = new List<IValidator>();
            this.responseNegotiators = new List<IResponseNegotiator>();
        }

        public IReadOnlyDictionary<(string verb, string path), CarterRoute> Routes
            => new ReadOnlyDictionary<(string verb, string path), CarterRoute>(this.routes);

        public IReadOnlyList<IStatusCodeHandler> StatusCodeHandlers 
            => this.statusCodeHandlers.AsReadOnly();
        
        public IReadOnlyList<IValidator> Validators 
            => this.validators.AsReadOnly();

        public IReadOnlyList<IResponseNegotiator> ResponseNegotiators
            => this.responseNegotiators.AsReadOnly();
        
        /// <summary>
        /// A global before handler which is invoked before all routes
        /// </summary>
        public Func<HttpContext, Task<bool>> Before { get; private set; }

        /// <summary>
        /// A global before handler which is invoked after all routes
        /// </summary>
        public Func<HttpContext, Task> After { get; private set; }

        public ICarterBootstrapper BeforeRequest(Func<HttpContext, Task<bool>> before)
        {
            this.Before = before;
            return this;
        }
        
        public ICarterBootstrapper AfterRequest(Func<HttpContext, Task<bool>> after)
        {
            this.After = after;
            return this;
        }
        
        public ICarterBootstrapper RegisterModules(params CarterModule[] modules)
        {
            foreach (var module in modules)
            {
                foreach (var route in module.Routes)
                {
                    this.routes.Add((route.Key.verb, route.Key.path), route.Value);
                }
            }
            return this;
        }
        
        public ICarterBootstrapper RegisterStatusCodeHandlers(params IStatusCodeHandler[] statusCodeHandlers)
        {
            this.statusCodeHandlers.AddRange(statusCodeHandlers);
            return this;
        }
       
        public ICarterBootstrapper RegisterValidators(params IValidator[] validators)
        {
            this.validators.AddRange(validators);
            return this;
        }
        
        public ICarterBootstrapper RegisterResponseNegotiators(params IResponseNegotiator[] negotiators)
        {
            this.responseNegotiators.AddRange(negotiators);
            return this;
        }
    }
}