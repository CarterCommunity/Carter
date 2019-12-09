namespace Carter.ModelBinding
{
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class DefaultJsonModelBinder : IModelBinder
    {
        public async Task<T> Bind<T>(HttpRequest request)
        {
            return await JsonSerializer.DeserializeAsync<T>(request.Body);
        }
    }
}
