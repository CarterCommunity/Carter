namespace Botwin.Response
{
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Net.Http.Headers;

    public static class ResponseExtensions
    {
        /// <summary>
        /// Executes content negotiation on current <see cref="HttpResponse"/>, utilizing an accepted media type if possible and defaulting to "application/json" if none found.
        /// </summary>
        /// <param name="response">Current <see cref="HttpResponse"/></param>
        /// <param name="obj">View model</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns><see cref="Task"/></returns>
        public static async Task Negotiate(this HttpResponse response, object obj, CancellationToken cancellationToken = default)
        {
            var negotiators = response.HttpContext.RequestServices.GetServices<IResponseNegotiator>();

            var accept = response.HttpContext.Request.GetTypedHeaders().Accept ?? Enumerable.Empty<MediaTypeHeaderValue>();

            var ordered = accept.OrderByDescending(x => x.Quality ?? 1);

            IResponseNegotiator negotiator = null;

            foreach (var acceptHeader in ordered)
            {
                negotiator = negotiators.FirstOrDefault(x => x.CanHandle(acceptHeader));
                if (negotiator != null)
                {
                    break;
                }
            }

            if (negotiator == null)
            {
                negotiator = negotiators.FirstOrDefault(x => x.CanHandle(new MediaTypeHeaderValue("application/json")));
            }

            await negotiator.Handle(response.HttpContext.Request, response, obj, cancellationToken);
        }

        /// <summary>
        /// Returns a Json response
        /// </summary>
        /// <param name="response">Current <see cref="HttpResponse"/></param>
        /// <param name="obj">View model</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns><see cref="Task"/></returns>
        public static async Task AsJson(this HttpResponse response, object obj, CancellationToken cancellationToken = default)
        {
            var negotiators = response.HttpContext.RequestServices.GetServices<IResponseNegotiator>();

            var negotiator = negotiators.FirstOrDefault(x => x.CanHandle(new MediaTypeHeaderValue("application/json")));

            await negotiator.Handle(response.HttpContext.Request, response, obj, cancellationToken);
        }

        /// <summary>
        /// Copy a stream into the response body
        /// </summary>
        /// <param name="response">Current <see cref="HttpResponse"/></param>
        /// <param name="stream">The <see cref="Stream"/> to copy from</param>
        /// <param name="contentType">The content type for the response</param>
        /// <param name="contentDisposition">The content disposition to allow file downloads</param>
        /// <returns></returns>
        public static async Task FromStream(this HttpResponse response, Stream stream, string contentType, ContentDisposition contentDisposition = null)
        {
            if (contentDisposition != null)
            {
                response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            }

            response.ContentType = contentType;

            await StreamCopyOperation.CopyToAsync(stream, response.Body, new long?(), 65536, response.HttpContext.RequestAborted);
        }
    }
}
