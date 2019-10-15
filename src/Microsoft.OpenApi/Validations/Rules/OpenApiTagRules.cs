// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiTag"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiTagRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiTag> TagRequiredFields =>
            new ValidationRule<OpenApiTag>(
                (context, tag) =>
                {
                    context.Enter("name");
                    if (tag.Name == null)
                    {
                        context.CreateError(nameof(TagRequiredFields),
                            String.Format(SRResource.Validation_FieldIsRequired, "name", "tag"));
                    }
                    context.Exit();
                });

        // add more rules
    }
}
