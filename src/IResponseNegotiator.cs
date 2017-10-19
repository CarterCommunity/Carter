namespace Botwin
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;

    public interface IResponseNegotiator
    {
        bool CanHandle(IList<MediaTypeHeaderValue> accept);

        Task Handle(HttpRequest req, HttpResponse res, object model, CancellationToken cancellationToken);
    }
}