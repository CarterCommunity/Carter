// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// Represents an Extensible Open API element.
    /// </summary>
    public interface IOpenApiExtensible : IOpenApiElement
    {
        /// <summary>
        /// Specification extensions.
        /// </summary>
        IDictionary<string, IOpenApiExtension> Extensions { get; set; }
    }
}