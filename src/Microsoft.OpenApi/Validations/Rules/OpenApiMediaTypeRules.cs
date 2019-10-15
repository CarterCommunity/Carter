// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiMediaType"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiMediaTypeRules
    {
        /// <summary>
        /// Validate the data matches with the given data type.
        /// </summary>
        public static ValidationRule<OpenApiMediaType> MediaTypeMismatchedDataType =>
            new ValidationRule<OpenApiMediaType>(
                (context, mediaType) =>
                {
                    // example
                    context.Enter("example");

                    if (mediaType.Example != null)
                    {
                        RuleHelpers.ValidateDataTypeMismatch(context, nameof(MediaTypeMismatchedDataType), mediaType.Example, mediaType.Schema);
                    }

                    context.Exit();


                    // enum
                    context.Enter("examples");

                    if (mediaType.Examples != null)
                    {
                        foreach (var key in mediaType.Examples.Keys)
                        {
                            if (mediaType.Examples[key] != null)
                            {
                                context.Enter(key);
                                context.Enter("value");
                                RuleHelpers.ValidateDataTypeMismatch(context, nameof(MediaTypeMismatchedDataType), mediaType.Examples[key]?.Value, mediaType.Schema);
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
