namespace Carter.ModelBinding;

using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        var validatorLocator = request.HttpContext.RequestServices.GetRequiredService<IValidatorLocator>();
        var validator = validatorLocator.GetValidator<T>();

        return validator == null
            ? throw new InvalidOperationException($"Cannot find validator for model of type '{typeof(T).Name}'")
            : validator.Validate(new ValidationContext<T>(model));
    }
    
    /// <summary>
    /// Performs validation on the specified <paramref name="model"/> instance
    /// </summary>
    /// <typeparam name="T">The type of the <paramref name="model"/> that is being validated</typeparam>
    /// <param name="request">Current <see cref="HttpRequest"/></param>
    /// <param name="model">The model instance that is being validated</param>
    /// <returns><see cref="Task{ValidationResult}"/></returns>
    public static async Task<ValidationResult> ValidateAsync<T>(this HttpRequest request, T model)
    {
        var validatorLocator = request.HttpContext.RequestServices.GetRequiredService<IValidatorLocator>();
        var validator = validatorLocator.GetValidator<T>();

        return validator == null
            ? throw new InvalidOperationException($"Cannot find validator for model of type '{typeof(T).Name}'")
            : await validator.ValidateAsync(new ValidationContext<T>(model));
    }

    /// <summary>
    /// Retrieve formatted validation errors
    /// </summary>
    /// <param name="result"><see cref="ValidationResult"/></param>
    /// <returns><see cref="IEnumerable{dynamic}"/> of property names and associated error messages</returns>
    public static IEnumerable<ModelError> GetFormattedErrors(this ValidationResult result)
    {
        return result.Errors.Select(x => new ModelError { PropertyName = x.PropertyName, ErrorMessage = x.ErrorMessage });
    }

    /// <summary>
    /// Retrieve formatted validation errors for validation problem result
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static Dictionary<string, string[]> GetValidationProblems(this ValidationResult result)
    {
        return result.Errors
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

}

public class ModelError
{
    public string PropertyName { get; set; }

    public string ErrorMessage { get; set; }
}