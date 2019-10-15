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
    /// Tag Object.
    /// </summary>
    public class OpenApiTag : IOpenApiSerializable, IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// The name of the tag.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A short description for the tag.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Additional external documentation for this tag.
        /// </summary>
        public OpenApiExternalDocs ExternalDocs { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public bool UnresolvedReference { get; set;}

        /// <summary>
        /// Reference.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v3.0
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

            writer.WriteValue(Name);
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v3.0
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

            await writer.WriteValueAsync(Name);
        }

        /// <summary>
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>
        public void SerializeAsV3WithoutReference(IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, Name);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // external docs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, e) => e.SerializeAsV3(w));

            // extensions.
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>
        public async Task SerializeAsV3WithoutReferenceAsync(IOpenApiWriter writer)
        {
            await writer.WriteStartObjectAsync();

            // name
            await writer.WritePropertyAsync(OpenApiConstants.Name, Name);

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);

            // external docs
            await writer.WriteOptionalObjectAsync(OpenApiConstants.ExternalDocs, ExternalDocs, async (w, e) => await e.SerializeAsV3Async(w));

            // extensions.
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v2.0
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

            writer.WriteValue(Name);
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v2.0
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

            await writer.WriteValueAsync(Name);
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, Name);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // external docs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, e) => e.SerializeAsV2(w));

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public async Task SerializeAsV2WithoutReferenceAsync(IOpenApiWriter writer)
        {
            await writer.WriteStartObjectAsync();

            // name
            await writer.WritePropertyAsync(OpenApiConstants.Name, Name);

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);

            // external docs
            await writer.WriteOptionalObjectAsync(OpenApiConstants.ExternalDocs, ExternalDocs, async (w, e) => await e.SerializeAsV2Async(w));

            // extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi2_0);

            await writer.WriteEndObjectAsync();
        }
    }
}