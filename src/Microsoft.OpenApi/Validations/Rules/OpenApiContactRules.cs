// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiContact"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiContactRules
    {
        /// <summary>
        /// Email field MUST be email address.
        /// </summary>
        public static ValidationRule<OpenApiContact> EmailMustBeEmailFormat =>
            new ValidationRule<OpenApiContact>(
                (context, item) =>
                {

                    context.Enter("email");
                    if (item != null && item.Email != null)
                    {
                        if (!item.Email.IsEmailAddress())
                        {
                            context.CreateError(nameof(EmailMustBeEmailFormat), 
                                String.Format(SRResource.Validation_StringMustBeEmailAddress, item.Email));
                        }
                    }
                    context.Exit();
                });

    }
}
