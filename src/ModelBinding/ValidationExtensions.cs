namespace Botwin.ModelBinding
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation.Results;

    public static class ValidationExtensions
    {
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