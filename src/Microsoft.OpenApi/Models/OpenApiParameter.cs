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
    /// Parameter Object.
    /// </summary>
    public class OpenApiParameter : IOpenApiSerializable, IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public bool UnresolvedReference { get; set;}

        /// <summary>
        /// Reference object.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// REQUIRED. The name of the parameter. Parameter names are case sensitive.
        /// If in is "path", the name field MUST correspond to the associated path segment from the path field in the Paths Object.
        /// If in is "header" and the name field is "Accept", "Content-Type" or "Authorization", the parameter definition SHALL be ignored.
        /// For all other cases, the name corresponds to the parameter name used by the in property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// REQUIRED. The location of the parameter.
        /// Possible values are "query", "header", "path" or "cookie".
        /// </summary>
        public ParameterLocation? In { get; set; }

        /// <summary>
        /// A brief description of the parameter. This could contain examples of use.
        /// CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Determines whether this parameter is mandatory.
        /// If the parameter location is "path", this property is REQUIRED and its value MUST be true.
        /// Otherwise, the property MAY be included and its default value is false.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Specifies that a parameter is deprecated and SHOULD be transitioned out of usage.
        /// </summary>
        public bool Deprecated { get; set; } = false;

        /// <summary>
        /// Sets the ability to pass empty-valued parameters.
        /// This is valid only for query parameters and allows sending a parameter with an empty value.
        /// Default value is false.
        /// If style is used, and if behavior is n/a (cannot be serialized),
        /// the value of allowEmptyValue SHALL be ignored.
        /// </summary>
        public bool AllowEmptyValue { get; set; } = false;

        /// <summary>
        /// Describes how the parameter value will be serialized depending on the type of the parameter value.
        /// Default values (based on value of in): for query - form; for path - simple; for header - simple;
        /// for cookie - form.
        /// </summary>
        public ParameterStyle? Style { get; set; }

        /// <summary>
        /// When this is true, parameter values of type array or object generate separate parameters
        /// for each value of the array or key-value pair of the map.
        /// For other types of parameters this property has no effect.
        /// When style is form, the default value is true.
        /// For all other styles, the default value is false.
        /// </summary>
        public bool Explode { get; set; }

        /// <summary>
        /// Determines whether the parameter value SHOULD allow reserved characters,
        /// as defined by RFC3986 :/?#[]@!$&amp;'()*+,;= to be included without percent-encoding.
        /// This property only applies to parameters with an in value of query.
        /// The default value is false.
        /// </summary>
        public bool AllowReserved { get; set; }

        /// <summary>
        /// The schema defining the type used for the parameter.
        /// </summary>
        public OpenApiSchema Schema { get; set; }

        /// <summary>
        /// Examples of the media type. Each example SHOULD contain a value
        /// in the correct format as specified in the parameter encoding.
        /// The examples object is mutually exclusive of the example object.
        /// Furthermore, if referencing a schema which contains an example,
        /// the examples value SHALL override the example provided by the schema.
        /// </summary>
        public IDictionary<string,OpenApiExample> Examples { get; set; } = new Dictionary<string,OpenApiExample>();

        /// <summary>
        /// Example of the media type. The example SHOULD match the specified schema and encoding properties
        /// if present. The example object is mutually exclusive of the examples object.
        /// Furthermore, if referencing a schema which contains an example,
        /// the example value SHALL override the example provided by the schema.
        /// To represent examples of media types that cannot naturally be represented in JSON or YAML,
        /// a string value can contain the example with escaping where necessary.
        /// </summary>
        public IOpenApiAny Example { get; set; }

        /// <summary>
        /// A map containing the representations for the parameter.
        /// The key is the media type and the value describes it.
        /// The map MUST only contain one entry.
        /// For more complex scenarios, the content property can define the media type and schema of the parameter.
        /// A parameter MUST contain either a schema property, or a content property, but not both.
        /// When example or examples are provided in conjunction with the schema object,
        /// the example MUST follow the prescribed serialization strategy for the parameter.
        /// </summary>
        public IDictionary<string, OpenApiMediaType> Content { get; set; } = new Dictionary<string, OpenApiMediaType>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize <see cref="OpenApiParameter"/> to Open Api v3.0
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
        /// Serialize <see cref="OpenApiParameter"/> to Open Api v3.0
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

            // name
            writer.WriteProperty(OpenApiConstants.Name, Name);

            // in
            writer.WriteProperty(OpenApiConstants.In, In?.GetDisplayName());

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
            writer.WriteProperty(OpenApiConstants.Explode, Explode, Style.HasValue && Style.Value == ParameterStyle.Form);

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

            // name
            await writer.WritePropertyAsync(OpenApiConstants.Name, Name);

            // in
            await writer.WritePropertyAsync(OpenApiConstants.In, In?.GetDisplayName());

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
            await writer.WritePropertyAsync(OpenApiConstants.Explode, Explode, Style.HasValue && Style.Value == ParameterStyle.Form);

            // allowReserved
            await writer.WritePropertyAsync(OpenApiConstants.AllowReserved, AllowReserved, false);

            // schema
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Schema, Schema, async (w, s) => await s.SerializeAsV3Async(w));

            // example
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Example, Example, async (w, s) => await w.WriteAnyAsync(s));

            // examples
            await writer.WriteOptionalMapAsync(OpenApiConstants.Examples, Examples, async (w, e) => await e.SerializeAsV3Async(w));

            // content
            await writer.WriteOptionalMapAsync(OpenApiConstants.Content, Content, async (w, c) => await c.SerializeAsV3Async(w));

            // extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiParameter"/> to Open Api v2.0
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
        /// Serialize <see cref="OpenApiParameter"/> to Open Api v2.0
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

            // in
            if (this is OpenApiFormDataParameter)
            {
                writer.WriteProperty(OpenApiConstants.In, "formData");
            }
            else if (this is OpenApiBodyParameter)
            {
                writer.WriteProperty(OpenApiConstants.In, "body");
            }
            else
            {
                writer.WriteProperty(OpenApiConstants.In, In?.GetDisplayName());
            }

            // name
            writer.WriteProperty(OpenApiConstants.Name, Name);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // required
            writer.WriteProperty(OpenApiConstants.Required, Required, false);

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);

            // schema
            if (this is OpenApiBodyParameter)
            {
                writer.WriteOptionalObject(OpenApiConstants.Schema, Schema, (w, s) => s.SerializeAsV2(w));
            }
            // In V2 parameter's type can't be a reference to a custom object schema or can't be of type object
            // So in that case map the type as string.
            else 
            if (Schema?.UnresolvedReference == true || Schema?.Type == "object")
            {
                writer.WriteProperty(OpenApiConstants.Type, "string");
            }
            else
            {
                // type
                // format
                // items
                // collectionFormat
                // default
                // maximum
                // exclusiveMaximum
                // minimum
                // exclusiveMinimum
                // maxLength
                // minLength
                // pattern
                // maxItems
                // minItems
                // uniqueItems
                // enum
                // multipleOf
                Schema?.WriteAsItemsProperties(writer);

                // allowEmptyValue
                writer.WriteProperty(OpenApiConstants.AllowEmptyValue, AllowEmptyValue, false);

                if (this.In == ParameterLocation.Query )
                {
                    if (this.Style == ParameterStyle.Form && this.Explode == true)
                    {
                        writer.WriteProperty("collectionFormat", "multi");
                    }
                    else if (this.Style == ParameterStyle.PipeDelimited)
                    {
                        writer.WriteProperty("collectionFormat", "pipes");
                    }
                    else if (this.Style == ParameterStyle.SpaceDelimited)
                    {
                        writer.WriteProperty("collectionFormat", "ssv");
                    }
                }
            }


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

            // in
            if (this is OpenApiFormDataParameter)
            {
                await writer.WritePropertyAsync(OpenApiConstants.In, "formData");
            }
            else if (this is OpenApiBodyParameter)
            {
                await writer.WritePropertyAsync(OpenApiConstants.In, "body");
            }
            else
            {
                await writer.WritePropertyAsync(OpenApiConstants.In, In?.GetDisplayName());
            }

            // name
            await writer.WritePropertyAsync(OpenApiConstants.Name, Name);

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);

            // required
            await writer.WritePropertyAsync(OpenApiConstants.Required, Required, false);

            // deprecated
            await writer.WritePropertyAsync(OpenApiConstants.Deprecated, Deprecated, false);

            // schema
            if (this is OpenApiBodyParameter)
            {
                await writer.WriteOptionalObjectAsync(OpenApiConstants.Schema, Schema, (w, s) => s.SerializeAsV2(w));
            }
            // In V2 parameter's type can't be a reference to a custom object schema or can't be of type object
            // So in that case map the type as string.
            else 
            if (Schema?.UnresolvedReference == true || Schema?.Type == "object")
            {
                await writer.WritePropertyAsync(OpenApiConstants.Type, "string");
            }
            else
            {
                // type
                // format
                // items
                // collectionFormat
                // default
                // maximum
                // exclusiveMaximum
                // minimum
                // exclusiveMinimum
                // maxLength
                // minLength
                // pattern
                // maxItems
                // minItems
                // uniqueItems
                // enum
                // multipleOf
                await Schema?.WriteAsItemsPropertiesAsync(writer);

                // allowEmptyValue
                await writer.WritePropertyAsync(OpenApiConstants.AllowEmptyValue, AllowEmptyValue, false);

                if (this.In == ParameterLocation.Query )
                {
                    if (this.Style == ParameterStyle.Form && this.Explode == true)
                    {
                        await writer.WritePropertyAsync("collectionFormat", "multi");
                    }
                    else if (this.Style == ParameterStyle.PipeDelimited)
                    {
                        await writer.WritePropertyAsync("collectionFormat", "pipes");
                    }
                    else if (this.Style == ParameterStyle.SpaceDelimited)
                    {
                        await writer.WritePropertyAsync("collectionFormat", "ssv");
                    }
                }
            }


            // extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi2_0);

            await writer.WriteEndObjectAsync();
        }

    }

    /// <summary>
    /// Body parameter class to propagate information needed for <see cref="OpenApiParameter.SerializeAsV2"/>
    /// </summary>
    internal class OpenApiBodyParameter : OpenApiParameter
    {
    }

    /// <summary>
    /// Form parameter class to propagate information needed for <see cref="OpenApiParameter.SerializeAsV2"/>
    /// </summary>
    internal class OpenApiFormDataParameter : OpenApiParameter
    {
    }
}