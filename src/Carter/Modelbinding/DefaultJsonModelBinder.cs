namespace Carter.ModelBinding
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class DefaultJsonModelBinder : IModelBinder
    {
        public async Task<T> Bind<T>(HttpRequest request)
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<T>(request.Body, new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
            }
            catch (JsonException)
            {
                return typeof(T).IsValueType == false ? Activator.CreateInstance<T>() : default;
            }
        }
    }
}
