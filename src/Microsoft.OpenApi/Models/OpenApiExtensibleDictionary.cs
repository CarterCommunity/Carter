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
    /// Generic dictionary type for Open API dictionary element.
    /// </summary>
    /// <typeparam name="T">The Open API element, <see cref="IOpenApiElement"/></typeparam>
    public abstract class OpenApiExtensibleDictionary<T> : Dictionary<string, T>,
        IOpenApiSerializable,
        IOpenApiExtensible
        where T : IOpenApiSerializable
    {
        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            foreach (var item in this)
            {
                writer.WriteRequiredObject(item.Key, item.Value, (w, p) => p.SerializeAsV3(w));
            }

            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Serialize to Open Api v3.0
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            foreach (var item in this)
            {
                await writer.WriteRequiredObjectAsync(item.Key, item.Value, async (w, p) => await p.SerializeAsV3Async(w));
            }

            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            foreach (var item in this)
            {
                writer.WriteRequiredObject(item.Key, item.Value, (w, p) => p.SerializeAsV2(w));
            }

            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Serialize to Open Api v2.0
        /// </summary>
        public async Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            foreach (var item in this)
            {
                await writer.WriteRequiredObjectAsync(item.Key, item.Value, async (w, p) => await p.SerializeAsV2Async(w));
            }

            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi2_0);

            await writer.WriteEndObjectAsync();
        }
    }
}