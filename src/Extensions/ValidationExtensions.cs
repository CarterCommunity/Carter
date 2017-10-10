namespace Botwin.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation.Results;

    public static class ValidationExtensions
    {
        public static IEnumerable<dynamic> GetFormattedErrors(this ValidationResult result)
        {
            return result.Errors.Select(x => new { x.PropertyName, x.ErrorMessage });
        }
    }
}