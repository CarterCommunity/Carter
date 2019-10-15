// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Contact Object.
    /// </summary>
    public class OpenApiContact : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// The identifying name of the contact person/organization.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The URL pointing to the contact information. MUST be in the format of a URL.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// The email address of the contact person/organization.
        /// MUST be in the format of an email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize <see cref="OpenApiContact"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            WriteInternal(writer, OpenApiSpecVersion.OpenApi3_0);
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiContact"/> to Open Api v3.0
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            await WriteInternalAsync(writer, OpenApiSpecVersion.OpenApi3_0);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiContact"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            WriteInternal(writer, OpenApiSpecVersion.OpenApi2_0);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiContact"/> to Open Api v2.0
        /// </summary>
        public async Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            await WriteInternalAsync(writer, OpenApiSpecVersion.OpenApi2_0);
        }

        private void WriteInternal(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, Name);

            // url
            writer.WriteProperty(OpenApiConstants.Url, Url?.OriginalString);

            // email
            writer.WriteProperty(OpenApiConstants.Email, Email);

            // extensions
            writer.WriteExtensions(Extensions, specVersion);

            writer.WriteEndObject();
        }
        
        private async Task WriteInternalAsync(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // name
            await writer.WritePropertyAsync(OpenApiConstants.Name, Name);

            // url
            await writer.WritePropertyAsync(OpenApiConstants.Url, Url?.OriginalString);

            // email
            await writer.WritePropertyAsync(OpenApiConstants.Email, Email);

            // extensions
            await writer.WriteExtensionsAsync(Extensions, specVersion);

            await writer.WriteEndObjectAsync();
        }
    }
}