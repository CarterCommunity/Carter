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
    /// XML Object.
    /// </summary>
    public class OpenApiXml : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// Replaces the name of the element/attribute used for the described schema property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The URI of the namespace definition. Value MUST be in the form of an absolute URI.
        /// </summary>
        public Uri Namespace { get; set; }

        /// <summary>
        /// The prefix to be used for the name
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Declares whether the property definition translates to an attribute instead of an element.
        /// Default value is false.
        /// </summary>
        public bool Attribute { get; set; }

        /// <summary>
        /// Signifies whether the array is wrapped.
        /// Default value is false.
        /// </summary>
        public bool Wrapped { get; set; }

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize <see cref="OpenApiXml"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            Write(writer, OpenApiSpecVersion.OpenApi3_0);
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiXml"/> to Open Api v3.0
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            await WriteAsync(writer, OpenApiSpecVersion.OpenApi3_0);

        }

        /// <summary>
        /// Serialize <see cref="OpenApiXml"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            Write(writer, OpenApiSpecVersion.OpenApi2_0);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiXml"/> to Open Api v2.0
        /// </summary>
        public async Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            await WriteAsync(writer, OpenApiSpecVersion.OpenApi2_0);
        }

        private void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, Name);

            // namespace
            writer.WriteProperty(OpenApiConstants.Namespace, Namespace?.AbsoluteUri);

            // prefix
            writer.WriteProperty(OpenApiConstants.Prefix, Prefix);

            // attribute
            writer.WriteProperty(OpenApiConstants.Attribute, Attribute, false);

            // wrapped
            writer.WriteProperty(OpenApiConstants.Wrapped, Wrapped, false);

            // extensions
            writer.WriteExtensions(Extensions, specVersion);

            writer.WriteEndObject();
        }
        
        private async Task WriteAsync(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // name
            await writer.WritePropertyAsync(OpenApiConstants.Name, Name);

            // namespace
            await writer.WritePropertyAsync(OpenApiConstants.Namespace, Namespace?.AbsoluteUri);

            // prefix
            await writer.WritePropertyAsync(OpenApiConstants.Prefix, Prefix);

            // attribute
            await writer.WritePropertyAsync(OpenApiConstants.Attribute, Attribute, false);

            // wrapped
            await writer.WritePropertyAsync(OpenApiConstants.Wrapped, Wrapped, false);

            // extensions
            await writer.WriteExtensionsAsync(Extensions, specVersion);

            await writer.WriteEndObjectAsync();
        }
    }
}