// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiParameter"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiParameterRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiParameter> ParameterRequiredFields =>
            new ValidationRule<OpenApiParameter>(
                (context, item) =>
                {
                    // name
                    context.Enter("name");
                    if (item.Name == null)
                    {
                        context.CreateError(nameof(ParameterRequiredFields),
                            String.Format(SRResource.Validation_FieldIsRequired, "name", "parameter"));
                    }
                    context.Exit();

                    // in
                    context.Enter("in");
                    if (item.In == null)
                    {
                        context.CreateError(nameof(ParameterRequiredFields), 
                            String.Format(SRResource.Validation_FieldIsRequired, "in", "parameter"));
                    }
                    context.Exit();
                });

        /// <summary>
        /// Validate the "required" field is true when "in" is path.
        /// </summary>
        public static ValidationRule<OpenApiParameter> RequiredMustBeTrueWhenInIsPath =>
            new ValidationRule<OpenApiParameter>(
                (context, item) =>
                {
                    // required
                    context.Enter("required");
                    if ( item.In == ParameterLocation.Path && !item.Required )
                    {
                        context.CreateError(
                            nameof(RequiredMustBeTrueWhenInIsPath),
                            "\"required\" must be true when parameter location is \"path\"");
                    }

                    context.Exit();
                });

        /// <summary>
        /// Validate the data matches with the given data type.
        /// </summary>
        public static ValidationRule<OpenApiParameter> ParameterMismatchedDataType =>
            new ValidationRule<OpenApiParameter>(
                (context, parameter) =>
                {
                    // example
                    context.Enter("example");

                    if (parameter.Example != null)
                    {
                        RuleHelpers.ValidateDataTypeMismatch(context, nameof(ParameterMismatchedDataType), parameter.Example, parameter.Schema);
                    }

                    context.Exit();

                    // examples
                    context.Enter("examples");

                    if (parameter.Examples != null)
                    {
                        foreach (var key in parameter.Examples.Keys)
                        {
                            if (parameter.Examples[key] != null)
                            {
                                context.Enter(key);
                                context.Enter("value");
                                RuleHelpers.ValidateDataTypeMismatch(context, nameof(ParameterMismatchedDataType), parameter.Examples[key]?.Value, parameter.Schema);
                                context.Exit();
                                context.Exit();
                            }
                        }
                    }

                    context.Exit();
                });

        // add more rule.
    }
}
