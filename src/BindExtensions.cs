namespace Botwin
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FluentValidation;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;

    public static class BindExtensions
    {
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();

        public static (ValidationResult ValidationResult, T Data) BindAndValidate<T>(this HttpRequest request)
        {
            var data = request.Bind<T>();
            if (data == null)
            {
                data = Activator.CreateInstance<T>();
            }

            var validatorType = request.HttpContext.RequestServices.GetService<IAssemblyProvider>()
                .GetAssembly()
                .GetTypes()
                .FirstOrDefault(t => t.GetTypeInfo().BaseType != null &&
                    t.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType &&
                    t.GetTypeInfo().BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>) &&
                    t.Name.Equals(typeof(T).Name + "Validator", StringComparison.OrdinalIgnoreCase));

            if (validatorType == null)
            {
                return (new ValidationResult(new[] { new ValidationFailure(typeof(T).Name, "No validator found") }), default(T));
            }

            IValidator validator = (IValidator)Activator.CreateInstance(validatorType);
            var result = validator.Validate(data);
            return (result, data);
        }

        public static T Bind<T>(this HttpRequest request)
        {
            using (var streamReader = new StreamReader(request.Body))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return JsonSerializer.Deserialize<T>(jsonTextReader);
            }
        }
    }
}