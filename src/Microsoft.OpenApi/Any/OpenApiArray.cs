// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Writers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API array.
    /// </summary>
    public class OpenApiArray : List<IOpenApiAny>, IOpenApiAny
    {
        /// <summary>
        /// The type of <see cref="IOpenApiAny"/>
        /// </summary>
        public AnyType AnyType { get; } = AnyType.Array;

        /// <summary>
        /// Write out contents of OpenApiArray to passed writer
        /// </summary>
        /// <param name="writer">Instance of JSON or YAML writer.</param>
        /// <param name="specVersion">Version of the OpenAPI specification that that will be output.</param>
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteStartArray();

            foreach (var item in this)
            {
                writer.WriteAny(item);
            }

            writer.WriteEndArray();

        }

        /// <summary>
        /// Write out contents of OpenApiArray to passed writer
        /// </summary>
        /// <param name="writer">Instance of JSON or YAML writer.</param>
        /// <param name="specVersion">Version of the OpenAPI specification that that will be output.</param>
        public async Task WriteAsync(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            await writer.WriteStartArrayAsync();

            foreach (var item in this)
            {
                await writer.WriteAnyAsync(item);
            }

            await writer.WriteEndArrayAsync();
        }
    }
}