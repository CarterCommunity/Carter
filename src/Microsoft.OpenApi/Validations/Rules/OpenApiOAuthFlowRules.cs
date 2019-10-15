// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiOAuthFlow"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiOAuthFlowRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiOAuthFlow> OAuthFlowRequiredFields =>
            new ValidationRule<OpenApiOAuthFlow>(
                (context, flow) =>
                {
                    // authorizationUrl
                    context.Enter("authorizationUrl");
                    if (flow.AuthorizationUrl == null)
                    {
                        context.CreateError(nameof(OAuthFlowRequiredFields),
                            String.Format(SRResource.Validation_FieldIsRequired, "authorizationUrl", "OAuth Flow"));
                    }
                    context.Exit();

                    // tokenUrl
                    context.Enter("tokenUrl");
                    if (flow.TokenUrl == null)
                    {
                        context.CreateError(nameof(OAuthFlowRequiredFields), 
                            String.Format(SRResource.Validation_FieldIsRequired, "tokenUrl", "OAuth Flow"));
                    }
                    context.Exit();

                    // scopes
                    context.Enter("scopes");
                    if (flow.Scopes == null)
                    {
                        context.CreateError(nameof(OAuthFlowRequiredFields), 
                            String.Format(SRResource.Validation_FieldIsRequired, "scopes", "OAuth Flow"));
                    }
                    context.Exit();
                });

        // add more rule.
    }
}
