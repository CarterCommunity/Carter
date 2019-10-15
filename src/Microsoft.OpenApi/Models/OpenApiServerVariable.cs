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
    /// Server Variable Object.
    /// </summary>
    public class OpenApiServerVariable : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// An optional description for the server variable. CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// REQUIRED. The default value to use for substitution, and to send, if an alternate value is not supplied.
        /// Unlike the Schema Object's default, this value MUST be provided by the consumer.
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// An enumeration of string values to be used if the substitution options are from a limited set.
        /// </summary>
        public List<string> Enum { get; set; } = new List<string>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize <see cref="OpenApiServerVariable"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // default
            writer.WriteProperty(OpenApiConstants.Default, Default);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // enums
            writer.WriteOptionalCollection(OpenApiConstants.Enum, Enum, (w, s) => w.WriteValue(s));

            // specification extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiServerVariable"/> to Open Api v3.0
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // default
            await writer.WritePropertyAsync(OpenApiConstants.Default, Default);

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);

            // enums
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Enum, Enum, async (w, s) => await w.WriteValueAsync(s));

            // specification extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiServerVariable"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // ServerVariable does not exist in V2.
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiServerVariable"/> to Open Api v2.0
        /// </summary>
        public Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            // ServerVariable does not exist in V2.
            return Task.CompletedTask;
        }
    }
}