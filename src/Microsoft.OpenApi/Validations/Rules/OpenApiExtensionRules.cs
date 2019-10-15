// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="IOpenApiExtensible"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiExtensibleRules
    {
        /// <summary>
        /// Extension name MUST start with "x-".
        /// </summary>
        public static ValidationRule<IOpenApiExtensible> ExtensionNameMustStartWithXDash =>
            new ValidationRule<IOpenApiExtensible>(
                (context, item) =>
                {
                    context.Enter("extensions");
                    foreach (var extensible in item.Extensions)
                    {
                        if (!extensible.Key.StartsWith("x-"))
                        {
                            context.CreateError(nameof(ExtensionNameMustStartWithXDash),
                                String.Format(SRResource.Validation_ExtensionNameMustBeginWithXDash, extensible.Key, context.PathString));
                        }
                    }
                    context.Exit();
                });
    }
}
