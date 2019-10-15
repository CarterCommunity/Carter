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
    /// Header Object.
    /// The Header Object follows the structure of the Parameter Object.
    /// </summary>
    public class OpenApiHeader : IOpenApiSerializable, IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public bool UnresolvedReference { get; set;}

        /// <summary>
        /// Reference pointer.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// A brief description of the header.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Determines whether this header is mandatory.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Specifies that a header is deprecated and SHOULD be transitioned out of usage.
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// Sets the ability to pass empty-valued headers.
        /// </summary>
        public bool AllowEmptyValue { get; set; }

        /// <summary>
        /// Describes how the header value will be serialized depending on the type of the header value.
        /// </summary>
        public ParameterStyle? Style { get; set; }

        /// <summary>
        /// When this is true, header values of type array or object generate separate parameters
        /// for each value of the array or key-value pair of the map.
        /// </summary>
        public bool Explode { get; set; }

        /// <summary>
        /// Determines whether the header value SHOULD allow reserved characters, as defined by RFC3986.
        /// </summary>
        public bool AllowReserved { get; set; }

        /// <summary>
        /// The schema defining the type used for the header.
        /// </summary>
        public OpenApiSchema Schema { get; set; }

        /// <summary>
        /// Example of the media type.
        /// </summary>
        public IOpenApiAny Example { get; set; }

        /// <summary>
        /// Examples of the media type.
        /// </summary>
        public IDictionary<string, OpenApiExample> Examples { get; set; } = new Dictionary<string, OpenApiExample>();

        /// <summary>
        /// A map containing the representations for the header.
        /// </summary>
        public IDictionary<string, OpenApiMediaType> Content { get; set; } = new Dictionary<string, OpenApiMediaType>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v3.0
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
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v3.0
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

            // required
            writer.WriteProperty(OpenApiConstants.Required, Required, false);

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);

            // allowEmptyValue
            writer.WriteProperty(OpenApiConstants.AllowEmptyValue, AllowEmptyValue, false);

            // style
            writer.WriteProperty(OpenApiConstants.Style, Style?.GetDisplayName());

            // explode
            writer.WriteProperty(OpenApiConstants.Explode, Explode, false);

            // allowReserved
            writer.WriteProperty(OpenApiConstants.AllowReserved, AllowReserved, false);

            // schema
            writer.WriteOptionalObject(OpenApiConstants.Schema, Schema, (w, s) => s.SerializeAsV3(w));

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, s) => w.WriteAny(s));

            // examples
            writer.WriteOptionalMap(OpenApiConstants.Examples, Examples, (w, e) => e.SerializeAsV3(w));

            // content
            writer.WriteOptionalMap(OpenApiConstants.Content, Content, (w, c) => c.SerializeAsV3(w));

            // extensions
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

            // required
            await writer.WritePropertyAsync(OpenApiConstants.Required, Required, false);

            // deprecated
            await writer.WritePropertyAsync(OpenApiConstants.Deprecated, Deprecated, false);

            // allowEmptyValue
            await writer.WritePropertyAsync(OpenApiConstants.AllowEmptyValue, AllowEmptyValue, false);

            // style
            await writer.WritePropertyAsync(OpenApiConstants.Style, Style?.GetDisplayName());

            // explode
            await writer.WritePropertyAsync(OpenApiConstants.Explode, Explode, false);

            // allowReserved
            await writer.WritePropertyAsync(OpenApiConstants.AllowReserved, AllowReserved, false);

            // schema
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Schema, Schema, (w, s) => s.SerializeAsV3(w));

            // example
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Example, Example, (w, s) => w.WriteAny(s));

            // examples
            await writer.WriteOptionalMapAsync(OpenApiConstants.Examples, Examples, (w, e) => e.SerializeAsV3(w));

            // content
            await writer.WriteOptionalMapAsync(OpenApiConstants.Content, Content, (w, c) => c.SerializeAsV3(w));

            // extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v2.0
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
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v2.0
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

            // required
            writer.WriteProperty(OpenApiConstants.Required, Required, false);

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);

            // allowEmptyValue
            writer.WriteProperty(OpenApiConstants.AllowEmptyValue, AllowEmptyValue, false);

            // style
            writer.WriteProperty(OpenApiConstants.Style, Style?.GetDisplayName());

            // explode
            writer.WriteProperty(OpenApiConstants.Explode, Explode, false);

            // allowReserved
            writer.WriteProperty(OpenApiConstants.AllowReserved, AllowReserved, false);

            // schema
            Schema?.WriteAsItemsProperties(writer);

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, s) => w.WriteAny(s));

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

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);

            // required
            await writer.WritePropertyAsync(OpenApiConstants.Required, Required, false);

            // deprecated
            await writer.WritePropertyAsync(OpenApiConstants.Deprecated, Deprecated, false);

            // allowEmptyValue
            await writer.WritePropertyAsync(OpenApiConstants.AllowEmptyValue, AllowEmptyValue, false);

            // style
            await writer.WritePropertyAsync(OpenApiConstants.Style, Style?.GetDisplayName());

            // explode
            await writer.WritePropertyAsync(OpenApiConstants.Explode, Explode, false);

            // allowReserved
            await writer.WritePropertyAsync(OpenApiConstants.AllowReserved, AllowReserved, false);

            // schema
            await Schema?.WriteAsItemsPropertiesAsync(writer);

            // example
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Example, Example, (w, s) => w.WriteAny(s));

            // extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi2_0);

            await writer.WriteEndObjectAsync();
        }
    }
}