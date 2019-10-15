// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Threading.Tasks;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API null.
    /// </summary>
    public class OpenApiNull : IOpenApiAny
    {
        /// <summary>
        /// The type of <see cref="IOpenApiAny"/>
        /// </summary>
        public AnyType AnyType { get; } = AnyType.Null;

        /// <summary>
        /// Write out null representation
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="specVersion">Version of the OpenAPI specification that that will be output.</param>
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteAny(this);
        }

        /// <summary>
        /// Write out null representation
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="specVersion">Version of the OpenAPI specification that that will be output.</param>
        public async Task WriteAsync(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            await writer.WriteAnyAsync(this);
        }
    }
}