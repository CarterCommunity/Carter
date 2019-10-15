// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// ExternalDocs object.
    /// </summary>
    public class OpenApiEncoding : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// The Content-Type for encoding a specific property.
        /// The value can be a specific media type (e.g. application/json),
        /// a wildcard media type (e.g. image/*), or a comma-separated list of the two types.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// A map allowing additional information to be provided as headers.
        /// </summary>
        public IDictionary<string, OpenApiHeader> Headers { get; set; } = new Dictionary<string, OpenApiHeader>();

        /// <summary>
        /// Describes how a specific property value will be serialized depending on its type.
        /// </summary>
        public ParameterStyle? Style { get; set; }

        /// <summary>
        /// When this is true, property values of type array or object generate separate parameters
        /// for each value of the array, or key-value-pair of the map. For other types of properties
        /// this property has no effect. When style is form, the default value is true.
        /// For all other styles, the default value is false.
        /// This property SHALL be ignored if the request body media type is not application/x-www-form-urlencoded.
        /// </summary>
        public bool? Explode { get; set; }

        /// <summary>
        /// Determines whether the parameter value SHOULD allow reserved characters,
        /// as defined by RFC3986 :/?#[]@!$&amp;'()*+,;= to be included without percent-encoding.
        /// The default value is false. This property SHALL be ignored
        /// if the request body media type is not application/x-www-form-urlencoded.
        /// </summary>
        public bool? AllowReserved { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();

            // contentType
            writer.WriteProperty(OpenApiConstants.ContentType, ContentType);

            // headers
            writer.WriteOptionalMap(OpenApiConstants.Headers, Headers, (w, h) => h.SerializeAsV3(w));

            // style
            writer.WriteProperty(OpenApiConstants.Style, Style?.GetDisplayName());

            // explode
            writer.WriteProperty(OpenApiConstants.Explode, Explode, false);

            // allowReserved
            writer.WriteProperty(OpenApiConstants.AllowReserved, AllowReserved, false);

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v3.0.
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            await writer.WriteStartObjectAsync();

            // contentType
            await writer.WritePropertyAsync(OpenApiConstants.ContentType, ContentType);

            // headers
            await writer.WriteOptionalMapAsync(OpenApiConstants.Headers, Headers, async (w, h) => await h.SerializeAsV3Async(w));

            // style
            await writer.WritePropertyAsync(OpenApiConstants.Style, Style?.GetDisplayName());

            // explode
            await writer.WritePropertyAsync(OpenApiConstants.Explode, Explode, false);

            // allowReserved
            await writer.WritePropertyAsync(OpenApiConstants.AllowReserved, AllowReserved, false);

            // extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // nothing here
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v2.0.
        /// </summary>
        public Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            return Task.CompletedTask;
        }
    }
}