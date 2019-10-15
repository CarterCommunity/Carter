// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// Helper methods to simplify creating validation rules
    /// </summary>
    public static class ValidationContextExtensions
    {
        /// <summary>
        /// Helper method to simplify validation rules
        /// </summary>
        public static void CreateError(this IValidationContext context, string ruleName, string message)
        {
            OpenApiValidatorError error = new OpenApiValidatorError(ruleName, context.PathString, message);
            context.AddError(error);
        }
    }
}
