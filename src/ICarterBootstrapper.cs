namespace Carter
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentValidation;
    using Microsoft.AspNetCore.Http;

    public interface ICarterBootstrapper
    {
        IReadOnlyDictionary<(string verb, string path), CarterRoute> Routes { get; }

        IReadOnlyList<IStatusCodeHandler> StatusCodeHandlers { get; }

        IReadOnlyList<IValidator> Validators { get; }

        IReadOnlyList<IResponseNegotiator> ResponseNegotiators { get; }

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