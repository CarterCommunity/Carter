namespace Botwin.Response
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Net.Http.Headers;

    public static class ResponseExtensions
    {
        /// <summary>
        /// Handles current response, utilizing an accepted media type if possible and defaulting to "application/json" if none found.
        /// </summary>
        /// <param name="response">Current <see cref="HttpResponse"/></param>
        /// <param name="obj">View model</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns><see cref="Task"/></returns>
        public static async Task Negotiate(this HttpResponse response, object obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            var negotiators = response.HttpContext.RequestServices.GetServices<IResponseNegotiator>();

            var accept = response.HttpContext.Request.GetTypedHeaders().Accept ?? new List<MediaTypeHeaderValue>();

            var negotiator = negotiators.FirstOrDefault(x => x.CanHandle(accept)) ?? negotiators.FirstOrDefault(x => x.CanHandle(new List<MediaTypeHeaderValue>() { new MediaTypeHeaderValue("application/json") }));

            await negotiator.Handle(response.HttpContext.Request, response, obj, cancellationToken);
        }

        /// <summary>
        /// Handles current response as Json data
        /// </summary>
        /// <param name="response">Current <see cref="HttpResponse"/></param>
        /// <param name="obj">View model</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns><see cref="Task"/></returns>
        public static async Task AsJson(this HttpResponse response, object obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            var negotiators = response.HttpContext.RequestServices.GetServices<IResponseNegotiator>();

            var negotiator = negotiators.FirstOrDefault(x => x.CanHandle(new List<MediaTypeHeaderValue>() { new MediaTypeHeaderValue("application/json") }));

            await negotiator.Handle(response.HttpContext.Request, response, obj, cancellationToken);
        }
    }
}