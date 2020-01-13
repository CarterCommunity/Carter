namespace Carter.ModelBinding
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public interface IModelBinder
    {
        Task<T> Bind<T>(HttpRequest request);
    }
}
