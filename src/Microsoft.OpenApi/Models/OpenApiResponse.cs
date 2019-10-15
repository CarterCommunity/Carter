// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Response object.
    /// </summary>
    public class OpenApiResponse : IOpenApiSerializable, IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. A short description of the response.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Maps a header name to its definition.
        /// </summary>
        public IDictionary<string, OpenApiHeader> Headers { get; set; } = new Dictionary<string, OpenApiHeader>();

        /// <summary>
        /// A map containing descriptions of potential response payloads.
        /// The key is a media type or media type range and the value describes it.
        /// </summary>
        public IDictionary<string, OpenApiMediaType> Content { get; set; } = new Dictionary<string, OpenApiMediaType>();

        /// <summary>
        /// A map of operations links that can be followed from the response.
        /// The key of the map is a short name for the link,
        /// following the naming constraints of the names for Component Objects.
        /// </summary>
        public IDictionary<string, OpenApiLink> Links { get; set; } = new Dictionary<string, OpenApiLink>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public bool UnresolvedReference { get; set;}

        /// <summary>
        /// Reference pointer.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (Reference != null)
            {
                Reference.SerializeAsV3(writer);
                return;
            }

            SerializeAsV3WithoutReference(writer);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v3.0.
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (Reference != null)
            {
                await Reference.SerializeAsV3Async(writer);
                return;
            }

            await SerializeAsV3WithoutReferenceAsync(writer);
        }

        /// <summary>
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>
        public void SerializeAsV3WithoutReference(IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // headers
            writer.WriteOptionalMap(OpenApiConstants.Headers, Headers, (w, h) => h.SerializeAsV3(w));

            // content
            writer.WriteOptionalMap(OpenApiConstants.Content, Content, (w, c) => c.SerializeAsV3(w));

            // links
            writer.WriteOptionalMap(OpenApiConstants.Links, Links, (w, l) => l.SerializeAsV3(w));

            // extension
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>
        public async Task SerializeAsV3WithoutReferenceAsync(IOpenApiWriter writer)
        {
            await writer.WriteStartObjectAsync();

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);

            // headers
            await writer.WriteOptionalMapAsync(OpenApiConstants.Headers, Headers, async (w, h) => await h.SerializeAsV3Async(w));

            // content
            await writer.WriteOptionalMapAsync(OpenApiConstants.Content, Content, async (w, c) => await c.SerializeAsV3Async(w));

            // links
            await writer.WriteOptionalMapAsync(OpenApiConstants.Links, Links, async (w, l) => await l.SerializeAsV3Async(w));

            // extension
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (Reference != null)
            {
                Reference.SerializeAsV2(writer);
                return;
            }

            SerializeAsV2WithoutReference(writer);
        }


        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v2.0.
        /// </summary>
        public async Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (Reference != null)
            {
                await Reference.SerializeAsV2Async(writer);
                return;
            }

            await SerializeAsV2WithoutReferenceAsync(writer);
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);
            if (Content != null)
            {
                var mediatype = Content.FirstOrDefault();
                if (mediatype.Value != null)
                {
                    // schema
                    writer.WriteOptionalObject(
                        OpenApiConstants.Schema,
                        mediatype.Value.Schema,
                        (w, s) => s.SerializeAsV2(w));

                    // examples
                    if (Content.Values.Any(m => m.Example != null))
                    {
                        writer.WritePropertyName(OpenApiConstants.Examples);
                        writer.WriteStartObject();

                        foreach (var mediaTypePair in Content)
                        {
                            if (mediaTypePair.Value.Example != null)
                            {
                                writer.WritePropertyName(mediaTypePair.Key);
                                writer.WriteAny(mediaTypePair.Value.Example);
                            }
                        }

                        writer.WriteEndObject();
                    }
                }
            }

            // headers
            writer.WriteOptionalMap(OpenApiConstants.Headers, Headers, (w, h) => h.SerializeAsV2(w));

            // extension
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public async Task SerializeAsV2WithoutReferenceAsync(IOpenApiWriter writer)
        {
            await writer.WriteStartObjectAsync();

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);
            if (Content != null)
            {
                var mediatype = Content.FirstOrDefault();
                if (mediatype.Value != null)
                {
                    // schema
                    await writer.WriteOptionalObjectAsync(
                        OpenApiConstants.Schema,
                        mediatype.Value.Schema,
                        async(w, s) => await s.SerializeAsV2Async(w));

                    // examples
                    if (Content.Values.Any(m => m.Example != null))
                    {
                        await writer.WritePropertyNameAsync(OpenApiConstants.Examples);
                        await writer.WriteStartObjectAsync();

                        foreach (var mediaTypePair in Content)
                        {
                            if (mediaTypePair.Value.Example != null)
                            {
                                await writer.WritePropertyNameAsync(mediaTypePair.Key);
                                await writer.WriteAnyAsync(mediaTypePair.Value.Example);
                            }
                        }

                        await writer.WriteEndObjectAsync();
                    }
                }
            }

            // headers
            await writer.WriteOptionalMapAsync(OpenApiConstants.Headers, Headers, async (w, h) => await h.SerializeAsV2Async(w));

            // extension
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi2_0);

            await writer.WriteEndObjectAsync();
        }

    }
}