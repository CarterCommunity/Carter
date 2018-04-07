namespace Carter.ModelBinding
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    public static class ValidationExtensions
    {
        /// <summary>
        /// Performs validation on the specified <paramref name="model"/> instance
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="model"/> that is being validated</typeparam>
        /// <param name="request">Current <see cref="HttpRequest"/></param>
        /// <param name="model">The model instance that is being validated</param>
        /// <returns><see cref="ValidationResult"/></returns>
        public static ValidationResult Validate<T>(this HttpRequest request, T model)
        {
            var validatorLocator = request.HttpContext.RequestServices.GetService<IValidatorLocator>();
            var validator = validatorLocator.GetValidator<T>();

            return validator == null
                ? new ValidationResult(new[] { new ValidationFailure(typeof(T).Name, "No validator found") })
                : validator.Validate(model);
        }

        /// <summary>
        /// Retrieve formatted validation errors
        /// </summary>
        /// <param name="result"><see cref="ValidationResult"/></param>
        /// <returns><see cref="IEnumerable{dynamic}"/> of property names and associated error messages</returns>
        public static IEnumerable<dynamic> GetFormattedErrors(this ValidationResult result)
        {
            return result.Errors.Select(x => new { x.PropertyName, x.ErrorMessage });
        }
    }
}