// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Media Type Object.
    /// </summary>
    public class OpenApiMediaType : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// The schema defining the type used for the request body.
        /// </summary>
        public OpenApiSchema Schema { get; set; }

        /// <summary>
        /// Example of the media type.
        /// The example object SHOULD be in the correct format as specified by the media type.
        /// </summary>
        public IOpenApiAny Example { get; set; }

        /// <summary>
        /// Examples of the media type.
        /// Each example object SHOULD match the media type and specified schema if present.
        /// </summary>
        public IDictionary<string, OpenApiExample> Examples { get; set; } = new Dictionary<string, OpenApiExample>();

        /// <summary>
        /// A map between a property name and its encoding information.
        /// The key, being the property name, MUST exist in the schema as a property.
        /// The encoding object SHALL only apply to requestBody objects
        /// when the media type is multipart or application/x-www-form-urlencoded.
        /// </summary>
        public IDictionary<string, OpenApiEncoding> Encoding { get; set; } = new Dictionary<string, OpenApiEncoding>();

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v3.0.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // schema
            writer.WriteOptionalObject(OpenApiConstants.Schema, Schema, (w, s) => s.SerializeAsV3(w));

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, e) => w.WriteAny(e));

            // examples
            writer.WriteOptionalMap(OpenApiConstants.Examples, Examples, (w, e) => e.SerializeAsV3(w));

            // encoding
            writer.WriteOptionalMap(OpenApiConstants.Encoding, Encoding, (w, e) => e.SerializeAsV3(w));

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v3.0.
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // schema
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Schema, Schema, async (w, s) => await s.SerializeAsV3Async(w));

            // example
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Example, Example, async (w, e) => await w.WriteAnyAsync(e));

            // examples
            await writer.WriteOptionalMapAsync(OpenApiConstants.Examples, Examples, async (w, e) => await e.SerializeAsV3Async(w));

            // encoding
            await writer.WriteOptionalMapAsync(OpenApiConstants.Encoding, Encoding, async (w, e) => await e.SerializeAsV3Async(w));

            // extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Media type does not exist in V2.
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v2.0.
        /// </summary>
        public Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            return Task.CompletedTask;
        }
    }
}