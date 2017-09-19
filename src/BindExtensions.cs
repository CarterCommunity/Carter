namespace Botwin
{
    using System;
    using System.Collections.Generic;
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

            var validatorLocator = request.HttpContext.RequestServices.GetService<IValidatorLocator>();

            var validator = validatorLocator.GetValidator<T>();

            if (validator == null)
            {
                return (new ValidationResult(new[] { new ValidationFailure(typeof(T).Name, "No validator found") }), default(T));
            }

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