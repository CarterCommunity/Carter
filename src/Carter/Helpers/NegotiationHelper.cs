namespace Carter.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Carter.Attributes;
using Carter.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

public static class NegotiationHelper
{
    /// <summary>
    /// Selects the most appropriate <see cref="IResponseNegotiator"/> for content negotiation based on the current <see cref="HttpContext"/>'s "Accept" headers, 
    /// or defaults to <see cref="DefaultJsonResponseNegotiator"/> if none match.
    /// </summary>
    /// <param name="httpContext">Current <see cref="HttpContext"/></param>
    /// <param name="negotiators">List of available <see cref="IResponseNegotiator"/> instances</param>
    /// <returns>The selected <see cref="IResponseNegotiator"/> for the response.</returns>
    public static IResponseNegotiator SelectNegotiator(HttpContext httpContext, List<IResponseNegotiator> negotiators)
    {
        IResponseNegotiator negotiator = null;

        MediaTypeHeaderValue.TryParseList(httpContext.Request.Headers["Accept"], out var accept);
        if (accept != null)
        {
            var ordered = accept.OrderByDescending(x => x.Quality ?? 1);

            foreach (var acceptHeader in ordered)
            {
                negotiator = negotiators.FirstOrDefault(x => x.CanHandle(acceptHeader));
                if (negotiator != null)
                {
                    break;
                }
            }
        }

        if (negotiator == null)
        {
            negotiator = negotiators.First(x => x.GetType() == typeof(DefaultJsonResponseNegotiator));
        }

        return negotiator;
    }
    
    public static bool IsTestNegotiator(IResponseNegotiator negotiator) 
        => negotiator.GetType().IsDefined(typeof(TestNegotiatorAttribute), inherit: true);
}
