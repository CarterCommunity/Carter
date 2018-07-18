namespace Carter
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentValidation;
    using Microsoft.AspNetCore.Http;

    public interface ICarterBootstrapper
    {
        Dictionary<(string verb, string path), CarterRoute> Routes { get; }

        List<IStatusCodeHandler> StatusCodeHandlers { get; }

        Dictionary<Type, IValidator> Validators { get; }

        List<IResponseNegotiator> ResponseNegotiators { get; }

        /// <summary>
        /// A global before handler which is invoked before all routes
        /// </summary>
        Func<HttpContext, Task<bool>> Before { get; }

        /// <summary>
        /// A global before handler which is invoked after all routes
        /// </summary>
        Func<HttpContext, Task> After { get; }

        ICarterBootstrapper RegisterModules(params CarterModule[] modules);

        ICarterBootstrapper RegisterStatusCodeHandlers(params IStatusCodeHandler[] statusCodeHandlers);

        ICarterBootstrapper RegisterValidators(params IValidator[] validators);

        ICarterBootstrapper RegisterResponseNegotiators(params IResponseNegotiator[] negotiators);
    }
}