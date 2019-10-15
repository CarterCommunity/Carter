// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiInfo"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiInfoRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiInfo> InfoRequiredFields =>
            new ValidationRule<OpenApiInfo>(
                (context, item) =>
                {

                    // title
                    context.Enter("title");
                    if (item.Title == null)
                    {
                        context.CreateError(nameof(InfoRequiredFields),
                            String.Format(SRResource.Validation_FieldIsRequired, "title", "info"));
                    }
                    context.Exit();

                    // version
                    context.Enter("version");
                    if (item.Version == null)
                    {
                        context.CreateError(nameof(InfoRequiredFields), 
                            String.Format(SRResource.Validation_FieldIsRequired, "version", "info"));
                    }
                    context.Exit();

                });

        // add more rule.
    }
}
