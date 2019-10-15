// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Discriminator object.
    /// </summary>
    public class OpenApiDiscriminator : IOpenApiSerializable
    {
        /// <summary>
        /// REQUIRED. The name of the property in the payload that will hold the discriminator value.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// An object to hold mappings between payload values and schema names or references.
        /// </summary>
        public IDictionary<string, string> Mapping { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // propertyName
            writer.WriteProperty(OpenApiConstants.PropertyName, PropertyName);

            // mapping
            writer.WriteOptionalMap(OpenApiConstants.Mapping, Mapping, (w, s) => w.WriteValue(s));

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v3.0
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // propertyName
            await writer.WritePropertyAsync(OpenApiConstants.PropertyName, PropertyName);

            // mapping
            await writer.WriteOptionalMapAsync(OpenApiConstants.Mapping, Mapping, (w, s) => w.WriteValue(s));

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v2.0
        /// </summary>
        public Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Discriminator object does not exist in V2.
        }
    }
}