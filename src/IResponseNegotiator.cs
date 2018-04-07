namespace Carter
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;

    public interface IResponseNegotiator
    {
        /// <summary>
        /// Checks whether any given media types are valid for the current response
        /// </summary>
        /// <param name="accept"><see cref="IList{MediaTypeHeaderValue}"/></param>
        /// <returns>True if any media types are acceptable, false if not</returns>
        bool CanHandle(MediaTypeHeaderValue accept);

        /// <summary>
        /// Handles the response utilizing the given view model
        /// </summary>
        /// <param name="req">Current <see cref="HttpRequest"/></param>
        /// <param name="res">Current <see cref="HttpResponse"/></param>
        /// <param name="model">View model</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns><see cref="Task"/></returns>
        Task Handle(HttpRequest req, HttpResponse res, object model, CancellationToken cancellationToken);
    }
}