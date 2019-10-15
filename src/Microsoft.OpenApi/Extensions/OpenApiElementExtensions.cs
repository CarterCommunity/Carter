// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Extension methods that apply across all OpenAPIElements
    /// </summary>
    public static class OpenApiElementExtensions
    {
        /// <summary>
        /// Validate element and all child elements
        /// </summary>
        /// <param name="element">Element to validate</param>
        /// <param name="ruleSet">Optional set of rules to use for validation</param>
        /// <returns>An IEnumerable of errors.  This function will never return null.</returns>
        public static IEnumerable<OpenApiError> Validate(this IOpenApiElement element, ValidationRuleSet ruleSet)
        {
            var validator = new OpenApiValidator(ruleSet);
            var walker = new OpenApiWalker(validator);
            walker.Walk(element);
            return validator.Errors;
        }
    }
}