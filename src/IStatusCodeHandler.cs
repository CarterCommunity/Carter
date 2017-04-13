namespace Botwin
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public interface IStatusCodeHandler
    {
        bool CanHandle(int statusCode);
        Task Handle(HttpContext ctx);
    }
}