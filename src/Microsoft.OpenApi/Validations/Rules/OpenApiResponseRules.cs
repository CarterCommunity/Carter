// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiResponse"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiResponseRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiResponse> ResponseRequiredFields =>
            new ValidationRule<OpenApiResponse>(
                (context, response) =>
                {
                    // description
                    context.Enter("description");
                    if (response.Description == null)
                    {
                        context.CreateError(nameof(ResponseRequiredFields),
                            String.Format(SRResource.Validation_FieldIsRequired, "description", "response"));
                    }
                    context.Exit();
                });

        // add more rule.
    }
}
