// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiPaths"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiPathsRules
    {

        /// <summary>
        /// A relative path to an individual endpoint. The field name MUST begin with a slash.
        /// </summary>
        public static ValidationRule<OpenApiPaths> PathNameMustBeginWithSlash =>
            new ValidationRule<OpenApiPaths>(
                (context, item) =>
                {
                    foreach (var pathName in item.Keys)
                    {
                        context.Enter(pathName);

                         if (pathName == null || !pathName.StartsWith("/"))
                        {
                            context.CreateError(nameof(PathNameMustBeginWithSlash),
                                string.Format(SRResource.Validation_PathItemMustBeginWithSlash, pathName));
                        }

                        context.Exit();
                    }
                });

        // add more rules
    }
}