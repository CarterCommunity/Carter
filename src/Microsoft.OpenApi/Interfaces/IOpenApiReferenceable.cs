// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// Represents an Open API element is referenceable.
    /// </summary>
    public interface IOpenApiReferenceable : IOpenApiSerializable
    {

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        bool UnresolvedReference { get; set;}

        /// <summary>
        /// Reference object.
        /// </summary>
        OpenApiReference Reference { get; set; }

        /// <summary>
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>
        void SerializeAsV3WithoutReference(IOpenApiWriter writer);

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        void SerializeAsV2WithoutReference(IOpenApiWriter writer);
    }
}