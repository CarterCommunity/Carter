// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Type of an <see cref="IOpenApiAny"/>
    /// </summary>
    public enum AnyType
    {
        /// <summary>
        /// Primitive.
        /// </summary>
        Primitive,

        /// <summary>
        /// Null.
        /// </summary>
        Null,

        /// <summary>
        /// Array.
        /// </summary>
        Array,

        /// <summary>
        /// Object.
        /// </summary>
        Object
    }
}