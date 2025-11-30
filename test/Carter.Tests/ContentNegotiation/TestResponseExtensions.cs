namespace Carter.Tests.ContentNegotiation;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Carter.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

public static class TestResponseExtensions
{
    public static Task Negotiate<T>(this HttpResponse response, T model, CancellationToken cancellationToken = default)
    {
        var negotiators = response.HttpContext.RequestServices
            .GetServices<IResponseNegotiator>()
            .ToList();
        
        var chosenNegotiator = NegotiationHelper.SelectNegotiator(response.HttpContext, negotiators);

        return chosenNegotiator.Handle(response.HttpContext.Request, response, model, cancellationToken);
    }
}
