namespace Carter.Tests.InternalRooms;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

internal static class NestedInternalResponseNegotiatorWrapper
{
    internal class NestedInternalResponseNegotiator: IResponseNegotiator
    {
        public bool CanHandle(MediaTypeHeaderValue accept)
        {
            return true;
        }
        
        public Task Handle<T>(HttpRequest req, HttpResponse res, T model, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

