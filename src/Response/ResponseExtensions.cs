namespace Botwin.Response
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
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
        /// <returns><<see cref="Task"/>/returns>
        public static async Task FromStream(this HttpResponse response, Stream source, string contentType, ContentDisposition contentDisposition = null)
        {
            long rangeStart;
            long rangeEnd;
            var contentLength = source.Length;

            response.Headers["AcceptRanges"] = "bytes";
            response.ContentType = contentType;

            if (contentDisposition != null)
            {
                response.Headers["Content-Disposition"] = contentDisposition.ToString();
            }

            // rangeHeader should be of the format "bytes=0-" or "bytes=0-12345" or "bytes=123-456"
            var rangeHeader = response.HttpContext.Request.Headers["Range"].FirstOrDefault();

            if (!string.IsNullOrEmpty(rangeHeader) && rangeHeader.Contains("="))
            {
                var rangeParts = rangeHeader.SplitOnFirst("=")[1].SplitOnFirst("-");
                rangeStart = long.Parse(rangeParts[0]);
                rangeEnd = rangeParts.Length == 2 && !string.IsNullOrEmpty(rangeParts[1])
                    ? int.Parse(rangeParts[1]) // the client requested a chunk
                    : contentLength - 1;

                if (rangeStart < 0 || rangeEnd > contentLength - 1)
                {
                    response.StatusCode = (int)HttpStatusCode.RequestedRangeNotSatisfiable;
                    await response.WriteAsync(string.Empty);
                }
                else
                {
                    response.Headers["ContentRange"] = $"bytes {rangeStart}-{rangeEnd}/{contentLength}";
                    response.StatusCode = (int)HttpStatusCode.PartialContent;
                    await GetResponseBodyDelegate(rangeStart, rangeEnd, contentLength, source, response.Body, response.HttpContext.RequestAborted);
                }
            }
            else
            {
                rangeStart = 0;
                rangeEnd = contentLength - 1;
                await GetResponseBodyDelegate(rangeStart, rangeEnd, contentLength, source, response.Body, response.HttpContext.RequestAborted);
            }
        }

        private static async Task GetResponseBodyDelegate(long rangeStart, long rangeEnd, long contentLength, Stream source, Stream destination, CancellationToken cancellationToken)
        {
            if (rangeStart == 0 && rangeEnd == contentLength - 1)
            {
                await StreamCopyOperation.CopyToAsync(source, destination, new long?(), 65536, cancellationToken);
            }
            else
            {
                if (!source.CanSeek)
                {
                    throw new InvalidOperationException("Sending Range Responses requires a seekable stream eg. FileStream or MemoryStream");
                }

                var totalBytesToSend = rangeEnd - rangeStart + 1;
                const int BufferSize = 0x1000;
                var buffer = new byte[BufferSize];
                var bytesRemaining = totalBytesToSend;

                source.Seek(rangeStart, SeekOrigin.Begin);
                while (bytesRemaining > 0)
                {
                    var count = bytesRemaining <= buffer.Length
                        ? await source.ReadAsync(buffer, 0, (int)Math.Min(bytesRemaining, int.MaxValue), cancellationToken)
                        : await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                    try
                    {
                        await destination.WriteAsync(buffer, 0, count, cancellationToken);
                        await destination.FlushAsync(cancellationToken);
                        bytesRemaining -= count;
                    }
                    catch (Exception httpException)
                    {
                        /* in Asp.Net we can call HttpResponseBase.IsClientConnected
                        * to see if the client broke off the connection
                        * and avoid trying to flush the response stream.
                        * instead I'll swallow the exception that IIS throws in this situation
                        * and rethrow anything else.*/
                        if (httpException.Message
                            == "An error occurred while communicating with the remote host. The error code is 0x80070057.") return;

                        throw;
                    }
                }
            }
        }

        public static string[] SplitOnFirst(this string strVal, string needle)
        {
            if (strVal == null)
            {
                return new string[0];
            }

            var pos = strVal.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + needle.Length) };
        }
    }
}
