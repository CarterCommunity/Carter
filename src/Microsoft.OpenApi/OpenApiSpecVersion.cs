// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Represents versions of OpenAPI specification.
    /// </summary>
    public enum OpenApiSpecVersion
    {
        /// <summary>
        /// Represents OpenAPI V2.0 spec
        /// </summary>
        OpenApi2_0,

        /// <summary>
        /// Represents all patches of OpenAPI V3.0 spec (e.g. 3.0.0, 3.0.1)
        /// </summary> 
        OpenApi3_0
    }
}