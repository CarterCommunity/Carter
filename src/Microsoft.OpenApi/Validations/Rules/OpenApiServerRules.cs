// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiServer"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiServerRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiServer> ServerRequiredFields =>
            new ValidationRule<OpenApiServer>(
                (context, server) =>
                {
                    context.Enter("url");
                    if (server.Url == null)
                    {
                        context.CreateError(nameof(ServerRequiredFields),
                            String.Format(SRResource.Validation_FieldIsRequired, "url", "server"));
                    }
                    context.Exit();
                });

        // add more rules
    }
}
