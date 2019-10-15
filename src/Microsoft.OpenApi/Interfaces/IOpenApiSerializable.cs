// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Threading.Tasks;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// Represents an Open API element that comes with serialzation functionality.
    /// </summary>
    public interface IOpenApiSerializable : IOpenApiElement
    {
        /// <summary>
        /// Serialize Open API element to v3.0.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void SerializeAsV3(IOpenApiWriter writer);

        /// <summary>
        /// Serialize Open API element to v2.0.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void SerializeAsV2(IOpenApiWriter writer);

        /// <summary>
        /// Serialize Open API element to v3.0.
        /// </summary>
        /// <param name="writer">The writer.</param>
        Task SerializeAsV3Async(IOpenApiWriter writer);

        /// <summary>
        /// Serialize Open API element to v2.0.
        /// </summary>
        /// <param name="writer">The writer.</param>
        Task SerializeAsV2Async(IOpenApiWriter writer);
    }
}