// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Attributes;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The reference type.
    /// </summary>
    public enum ReferenceType
    {
        /// <summary>
        /// Schema item.
        /// </summary>
        [Display("schemas")] Schema,

        /// <summary>
        /// Responses item.
        /// </summary>
        [Display("responses")] Response,

        /// <summary>
        /// Parameters item.
        /// </summary>
        [Display("parameters")] Parameter,

        /// <summary>
        /// Examples item.
        /// </summary>
        [Display("examples")] Example,

        /// <summary>
        /// RequestBodies item.
        /// </summary>
        [Display("requestBodies")] RequestBody,

        /// <summary>
        /// Headers item.
        /// </summary>
        [Display("headers")] Header,

        /// <summary>
        /// SecuritySchemes item.
        /// </summary>
        [Display("securitySchemes")] SecurityScheme,

        /// <summary>
        /// Links item.
        /// </summary>
        [Display("links")] Link,

        /// <summary>
        /// Callbacks item.
        /// </summary>
        [Display("callbacks")] Callback,

        /// <summary>
        /// Tags item.
        /// </summary>
        [Display("tags")] Tag
    }
}