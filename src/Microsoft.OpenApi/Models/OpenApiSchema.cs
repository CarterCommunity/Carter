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
    /// Schema Object.
    /// </summary>
    public class OpenApiSchema : IOpenApiSerializable, IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// Follow JSON Schema definition. Short text providing information about the data.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Value MUST be a string. Multiple types via an array are not supported.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// While relying on JSON Schema's defined formats,
        /// the OAS offers a few additional predefined formats.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public decimal? Maximum { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public bool? ExclusiveMaximum { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public decimal? Minimum { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public bool? ExclusiveMinimum { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public int? MinLength { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// This string SHOULD be a valid regular expression, according to the ECMA 262 regular expression dialect
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public decimal? MultipleOf { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// The default value represents what would be assumed by the consumer of the input as the value of the schema if one is not provided.
        /// Unlike JSON Schema, the value MUST conform to the defined type for the Schema Object defined at the same level.
        /// For example, if type is string, then default can be "foo" but cannot be 1.
        /// </summary>
        public IOpenApiAny Default { get; set; }

        /// <summary>
        /// Relevant only for Schema "properties" definitions. Declares the property as "read only".
        /// This means that it MAY be sent as part of a response but SHOULD NOT be sent as part of the request.
        /// If the property is marked as readOnly being true and is in the required list,
        /// the required will take effect on the response only.
        /// A property MUST NOT be marked as both readOnly and writeOnly being true.
        /// Default value is false.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Relevant only for Schema "properties" definitions. Declares the property as "write only".
        /// Therefore, it MAY be sent as part of a request but SHOULD NOT be sent as part of the response.
        /// If the property is marked as writeOnly being true and is in the required list,
        /// the required will take effect on the request only.
        /// A property MUST NOT be marked as both readOnly and writeOnly being true.
        /// Default value is false.
        /// </summary>
        public bool WriteOnly { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public IList<OpenApiSchema> AllOf { get; set; } = new List<OpenApiSchema>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public IList<OpenApiSchema> OneOf { get; set; } = new List<OpenApiSchema>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public IList<OpenApiSchema> AnyOf { get; set; } = new List<OpenApiSchema>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public OpenApiSchema Not { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public ISet<string> Required { get; set; } = new HashSet<string>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Value MUST be an object and not an array. Inline or referenced schema MUST be of a Schema Object
        /// and not a standard JSON Schema. items MUST be present if the type is array.
        /// </summary>
        public OpenApiSchema Items { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public int? MaxItems { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public int? MinItems { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public bool? UniqueItems { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Property definitions MUST be a Schema Object and not a standard JSON Schema (inline or referenced).
        /// </summary>
        public IDictionary<string, OpenApiSchema> Properties { get; set; } = new Dictionary<string, OpenApiSchema>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public int? MaxProperties { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public int? MinProperties { get; set; }

        /// <summary>
        /// Indicates if the schema can contain properties other than those defined by the properties map.
        /// </summary>
        public bool AdditionalPropertiesAllowed { get; set; } = true;

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Value can be boolean or object. Inline or referenced schema
        /// MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public OpenApiSchema AdditionalProperties { get; set; }


        /// <summary>
        /// Adds support for polymorphism. The discriminator is an object name that is used to differentiate
        /// between other schemas which may satisfy the payload description.
        /// </summary>
        public OpenApiDiscriminator Discriminator { get; set; }

        /// <summary>
        /// A free-form property to include an example of an instance for this schema.
        /// To represent examples that cannot be naturally represented in JSON or YAML,
        /// a string value can be used to contain the example with escaping where necessary.
        /// </summary>
        public IOpenApiAny Example { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public IList<IOpenApiAny> Enum { get; set; } = new List<IOpenApiAny>();

        /// <summary>
        /// Allows sending a null value for the defined schema. Default value is false.
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// Additional external documentation for this schema.
        /// </summary>
        public OpenApiExternalDocs ExternalDocs { get; set; }

        /// <summary>
        /// Specifies that a schema is deprecated and SHOULD be transitioned out of usage.
        /// Default value is false.
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// This MAY be used only on properties schemas. It has no effect on root schemas.
        /// Adds additional metadata to describe the XML representation of this property.
        /// </summary>
        public OpenApiXml Xml { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Indicates object is a placeholder reference to an actual object and does not contain valid data.
        /// </summary>
        public bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference object.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiSchema"/> to Open Api v3.0
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
        /// Serialize <see cref="OpenApiSchema"/> to Open Api v3.0
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

            // title
            writer.WriteProperty(OpenApiConstants.Title, Title);

            // multipleOf
            writer.WriteProperty(OpenApiConstants.MultipleOf, MultipleOf);

            // maximum
            writer.WriteProperty(OpenApiConstants.Maximum, Maximum);

            // exclusiveMaximum
            writer.WriteProperty(OpenApiConstants.ExclusiveMaximum, ExclusiveMaximum);

            // minimum
            writer.WriteProperty(OpenApiConstants.Minimum, Minimum);

            // exclusiveMinimum
            writer.WriteProperty(OpenApiConstants.ExclusiveMinimum, ExclusiveMinimum);

            // maxLength
            writer.WriteProperty(OpenApiConstants.MaxLength, MaxLength);

            // minLength
            writer.WriteProperty(OpenApiConstants.MinLength, MinLength);

            // pattern
            writer.WriteProperty(OpenApiConstants.Pattern, Pattern);

            // maxItems
            writer.WriteProperty(OpenApiConstants.MaxItems, MaxItems);

            // minItems
            writer.WriteProperty(OpenApiConstants.MinItems, MinItems);

            // uniqueItems
            writer.WriteProperty(OpenApiConstants.UniqueItems, UniqueItems);

            // maxProperties
            writer.WriteProperty(OpenApiConstants.MaxProperties, MaxProperties);

            // minProperties
            writer.WriteProperty(OpenApiConstants.MinProperties, MinProperties);

            // required
            writer.WriteOptionalCollection(OpenApiConstants.Required, Required, (w, s) => w.WriteValue(s));

            // enum
            writer.WriteOptionalCollection(OpenApiConstants.Enum, Enum, (nodeWriter, s) => nodeWriter.WriteAny(s));

            // type
            writer.WriteProperty(OpenApiConstants.Type, Type);

            // allOf
            writer.WriteOptionalCollection(OpenApiConstants.AllOf, AllOf, (w, s) => s.SerializeAsV3(w));

            // anyOf
            writer.WriteOptionalCollection(OpenApiConstants.AnyOf, AnyOf, (w, s) => s.SerializeAsV3(w));

            // oneOf
            writer.WriteOptionalCollection(OpenApiConstants.OneOf, OneOf, (w, s) => s.SerializeAsV3(w));

            // not
            writer.WriteOptionalObject(OpenApiConstants.Not, Not, (w, s) => s.SerializeAsV3(w));

            // items
            writer.WriteOptionalObject(OpenApiConstants.Items, Items, (w, s) => s.SerializeAsV3(w));

            // properties
            writer.WriteOptionalMap(OpenApiConstants.Properties, Properties, (w, s) => s.SerializeAsV3(w));

            // additionalProperties
            if (AdditionalPropertiesAllowed)
            {
                writer.WriteOptionalObject(
                    OpenApiConstants.AdditionalProperties,
                    AdditionalProperties,
                    (w, s) => s.SerializeAsV3(w));
            }
            else
            {
                writer.WriteProperty(OpenApiConstants.AdditionalProperties, AdditionalPropertiesAllowed);
            }

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // format
            writer.WriteProperty(OpenApiConstants.Format, Format);

            // default
            writer.WriteOptionalObject(OpenApiConstants.Default, Default, (w, d) => w.WriteAny(d));

            // nullable
            writer.WriteProperty(OpenApiConstants.Nullable, Nullable, false);

            // discriminator
            writer.WriteOptionalObject(OpenApiConstants.Discriminator, Discriminator, (w, s) => s.SerializeAsV3(w));

            // readOnly
            writer.WriteProperty(OpenApiConstants.ReadOnly, ReadOnly, false);

            // writeOnly
            writer.WriteProperty(OpenApiConstants.WriteOnly, WriteOnly, false);

            // xml
            writer.WriteOptionalObject(OpenApiConstants.Xml, Xml, (w, s) => s.SerializeAsV2(w));

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, s) => s.SerializeAsV3(w));

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, e) => w.WriteAny(e));

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);

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

            // title
            await writer.WritePropertyAsync(OpenApiConstants.Title, Title);

            // multipleOf
            await writer.WritePropertyAsync(OpenApiConstants.MultipleOf, MultipleOf);

            // maximum
            await writer.WritePropertyAsync(OpenApiConstants.Maximum, Maximum);

            // exclusiveMaximum
            await writer.WritePropertyAsync(OpenApiConstants.ExclusiveMaximum, ExclusiveMaximum);

            // minimum
            await writer.WritePropertyAsync(OpenApiConstants.Minimum, Minimum);

            // exclusiveMinimum
            await writer.WritePropertyAsync(OpenApiConstants.ExclusiveMinimum, ExclusiveMinimum);

            // maxLength
            await writer.WritePropertyAsync(OpenApiConstants.MaxLength, MaxLength);

            // minLength
            await writer.WritePropertyAsync(OpenApiConstants.MinLength, MinLength);

            // pattern
            await writer.WritePropertyAsync(OpenApiConstants.Pattern, Pattern);

            // maxItems
            await writer.WritePropertyAsync(OpenApiConstants.MaxItems, MaxItems);

            // minItems
            await writer.WritePropertyAsync(OpenApiConstants.MinItems, MinItems);

            // uniqueItems
            await writer.WritePropertyAsync(OpenApiConstants.UniqueItems, UniqueItems);

            // maxProperties
            await writer.WritePropertyAsync(OpenApiConstants.MaxProperties, MaxProperties);

            // minProperties
            await writer.WritePropertyAsync(OpenApiConstants.MinProperties, MinProperties);

            // required
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Required, Required, async (w, s) => await w.WriteValueAsync(s));

            // enum
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Enum, Enum, async (nodeWriter, s) => await nodeWriter.WriteAnyAsync(s));

            // type
            await writer.WritePropertyAsync(OpenApiConstants.Type, Type);

            // allOf
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.AllOf, AllOf, async (w, s) => await s.SerializeAsV3Async(w));

            // anyOf
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.AnyOf, AnyOf, async (w, s) => await s.SerializeAsV3Async(w));

            // oneOf
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.OneOf, OneOf, async (w, s) => await s.SerializeAsV3Async(w));

            // not
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Not, Not, async (w, s) => await s.SerializeAsV3Async(w));

            // items
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Items, Items, async (w, s) => await s.SerializeAsV3Async(w));

            // properties
            await writer.WriteOptionalMapAsync(OpenApiConstants.Properties, Properties, async (w, s) => await s.SerializeAsV3Async(w));

            // additionalProperties
            if (AdditionalPropertiesAllowed)
            {
                await writer.WriteOptionalObjectAsync(
                    OpenApiConstants.AdditionalProperties,
                    AdditionalProperties,
                    async (w, s) => await s.SerializeAsV3Async(w));
            }
            else
            {
                await writer.WritePropertyAsync(OpenApiConstants.AdditionalProperties, AdditionalPropertiesAllowed);
            }

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);

            // format
            await writer.WritePropertyAsync(OpenApiConstants.Format, Format);

            // default
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Default, Default, async (w, d) => await w.WriteAnyAsync(d));

            // nullable
            await writer.WritePropertyAsync(OpenApiConstants.Nullable, Nullable, false);

            // discriminator
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Discriminator, Discriminator, async (w, s) => await s.SerializeAsV3Async(w));

            // readOnly
            await writer.WritePropertyAsync(OpenApiConstants.ReadOnly, ReadOnly, false);

            // writeOnly
            await writer.WritePropertyAsync(OpenApiConstants.WriteOnly, WriteOnly, false);

            // xml
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Xml, Xml, async (w, s) => await s.SerializeAsV2Async(w));

            // externalDocs
            await writer.WriteOptionalObjectAsync(OpenApiConstants.ExternalDocs, ExternalDocs, async (w, s) => await s.SerializeAsV3Async(w));

            // example
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Example, Example, async (w, e) => await w.WriteAnyAsync(e));

            // deprecated
            await writer.WritePropertyAsync(OpenApiConstants.Deprecated, Deprecated, false);

            // extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSchema"/> to Open Api v2.0
        /// </summary>
        public async Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            await SerializeAsV2Async(writer: writer, parentRequiredProperties: new HashSet<string>(), propertyName: null);
        }


        /// <summary>
        /// Serialize <see cref="OpenApiSchema"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            SerializeAsV2(writer: writer, parentRequiredProperties: new HashSet<string>(), propertyName: null);
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
        {
            SerializeAsV2WithoutReference(
                writer: writer,
                parentRequiredProperties: new HashSet<string>(),
                propertyName: null);
        }
        
        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public async Task SerializeAsV2WithoutReferenceAsync(IOpenApiWriter writer)
        {
            await SerializeAsV2WithoutReferenceAsync(
                writer: writer,
                parentRequiredProperties: new HashSet<string>(),
                propertyName: null);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSchema"/> to Open Api v2.0 and handles not marking the provided property 
        /// as readonly if its included in the provided list of required properties of parent schema.
        /// </summary>
        /// <param name="writer">The open api writer.</param>
        /// <param name="parentRequiredProperties">The list of required properties in parent schema.</param>
        /// <param name="propertyName">The property name that will be serialized.</param>
        internal void SerializeAsV2(
            IOpenApiWriter writer,
            ISet<string> parentRequiredProperties,
            string propertyName)
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

            if (parentRequiredProperties == null)
            {
                parentRequiredProperties = new HashSet<string>();
            }

            SerializeAsV2WithoutReference(writer, parentRequiredProperties, propertyName);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSchema"/> to Open Api v2.0 and handles not marking the provided property 
        /// as readonly if its included in the provided list of required properties of parent schema.
        /// </summary>
        /// <param name="writer">The open api writer.</param>
        /// <param name="parentRequiredProperties">The list of required properties in parent schema.</param>
        /// <param name="propertyName">The property name that will be serialized.</param>
        internal async Task SerializeAsV2Async(
            IOpenApiWriter writer,
            ISet<string> parentRequiredProperties,
            string propertyName)
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

            if (parentRequiredProperties == null)
            {
                parentRequiredProperties = new HashSet<string>();
            }

            await SerializeAsV2WithoutReferenceAsync(writer, parentRequiredProperties, propertyName);
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference and handles not marking the provided property 
        /// as readonly if its included in the provided list of required properties of parent schema.
        /// </summary>
        /// <param name="writer">The open api writer.</param>
        /// <param name="parentRequiredProperties">The list of required properties in parent schema.</param>
        /// <param name="propertyName">The property name that will be serialized.</param>
        internal void SerializeAsV2WithoutReference(
            IOpenApiWriter writer,
            ISet<string> parentRequiredProperties,
            string propertyName)
        {
            writer.WriteStartObject();
            WriteAsSchemaProperties(writer, parentRequiredProperties, propertyName);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference and handles not marking the provided property 
        /// as readonly if its included in the provided list of required properties of parent schema.
        /// </summary>
        /// <param name="writer">The open api writer.</param>
        /// <param name="parentRequiredProperties">The list of required properties in parent schema.</param>
        /// <param name="propertyName">The property name that will be serialized.</param>
        internal async Task SerializeAsV2WithoutReferenceAsync(
            IOpenApiWriter writer,
            ISet<string> parentRequiredProperties,
            string propertyName)
        {
            await writer.WriteStartObjectAsync();
            await WriteAsSchemaPropertiesAsync(writer, parentRequiredProperties, propertyName);
            await writer.WriteEndObjectAsync();
        }

        internal void WriteAsItemsProperties(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            // type
            writer.WriteProperty(OpenApiConstants.Type, Type);

            // format
            writer.WriteProperty(OpenApiConstants.Format, Format);

            // items
            writer.WriteOptionalObject(OpenApiConstants.Items, Items, (w, s) => s.SerializeAsV2(w));

            // collectionFormat
            // We need information from style in parameter to populate this.
            // The best effort we can make is to pull this information from the first parameter
            // that leverages this schema. However, that in itself may not be as simple
            // as the schema directly under parameter might be referencing one in the Components,
            // so we will need to do a full scan of the object before we can write the value for
            // this property. This is not supported yet, so we will skip this property at the moment.

            // default
            writer.WriteOptionalObject(OpenApiConstants.Default, Default, (w, d) => w.WriteAny(d));

            // maximum
            writer.WriteProperty(OpenApiConstants.Maximum, Maximum);

            // exclusiveMaximum
            writer.WriteProperty(OpenApiConstants.ExclusiveMaximum, ExclusiveMaximum);

            // minimum
            writer.WriteProperty(OpenApiConstants.Minimum, Minimum);

            // exclusiveMinimum
            writer.WriteProperty(OpenApiConstants.ExclusiveMinimum, ExclusiveMinimum);

            // maxLength
            writer.WriteProperty(OpenApiConstants.MaxLength, MaxLength);

            // minLength
            writer.WriteProperty(OpenApiConstants.MinLength, MinLength);

            // pattern
            writer.WriteProperty(OpenApiConstants.Pattern, Pattern);

            // maxItems
            writer.WriteProperty(OpenApiConstants.MaxItems, MaxItems);

            // minItems
            writer.WriteProperty(OpenApiConstants.MinItems, MinItems);

            // enum
            writer.WriteOptionalCollection(OpenApiConstants.Enum, Enum, (w, s) => w.WriteAny(s));

            // multipleOf
            writer.WriteProperty(OpenApiConstants.MultipleOf, MultipleOf);

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);
        }

        internal async Task WriteAsItemsPropertiesAsync(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            // type
            await writer.WritePropertyAsync(OpenApiConstants.Type, Type);

            // format
            await writer.WritePropertyAsync(OpenApiConstants.Format, Format);

            // items
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Items, Items, async (w, s) => await s.SerializeAsV2Async(w));

            // collectionFormat
            // We need information from style in parameter to populate this.
            // The best effort we can make is to pull this information from the first parameter
            // that leverages this schema. However, that in itself may not be as simple
            // as the schema directly under parameter might be referencing one in the Components,
            // so we will need to do a full scan of the object before we can write the value for
            // this property. This is not supported yet, so we will skip this property at the moment.

            // default
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Default, Default, async (w, d) => await w.WriteAnyAsync(d));

            // maximum
            await writer.WritePropertyAsync(OpenApiConstants.Maximum, Maximum);

            // exclusiveMaximum
            await writer.WritePropertyAsync(OpenApiConstants.ExclusiveMaximum, ExclusiveMaximum);

            // minimum
            await writer.WritePropertyAsync(OpenApiConstants.Minimum, Minimum);

            // exclusiveMinimum
            await writer.WritePropertyAsync(OpenApiConstants.ExclusiveMinimum, ExclusiveMinimum);

            // maxLength
            await writer.WritePropertyAsync(OpenApiConstants.MaxLength, MaxLength);

            // minLength
            await writer.WritePropertyAsync(OpenApiConstants.MinLength, MinLength);

            // pattern
            await writer.WritePropertyAsync(OpenApiConstants.Pattern, Pattern);

            // maxItems
            await writer.WritePropertyAsync(OpenApiConstants.MaxItems, MaxItems);

            // minItems
            await writer.WritePropertyAsync(OpenApiConstants.MinItems, MinItems);

            // enum
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Enum, Enum, async (w, s) => await w.WriteAnyAsync(s));

            // multipleOf
            await writer.WritePropertyAsync(OpenApiConstants.MultipleOf, MultipleOf);

            // extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi2_0);
        }


        internal void WriteAsSchemaProperties(
            IOpenApiWriter writer,
            ISet<string> parentRequiredProperties,
            string propertyName)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            // format
            writer.WriteProperty(OpenApiConstants.Format, Format);

            // title
            writer.WriteProperty(OpenApiConstants.Title, Title);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // default
            writer.WriteOptionalObject(OpenApiConstants.Default, Default, (w, d) => w.WriteAny(d));

            // multipleOf
            writer.WriteProperty(OpenApiConstants.MultipleOf, MultipleOf);

            // maximum
            writer.WriteProperty(OpenApiConstants.Maximum, Maximum);

            // exclusiveMaximum
            writer.WriteProperty(OpenApiConstants.ExclusiveMaximum, ExclusiveMaximum);

            // minimum
            writer.WriteProperty(OpenApiConstants.Minimum, Minimum);

            // exclusiveMinimum
            writer.WriteProperty(OpenApiConstants.ExclusiveMinimum, ExclusiveMinimum);

            // maxLength
            writer.WriteProperty(OpenApiConstants.MaxLength, MaxLength);

            // minLength
            writer.WriteProperty(OpenApiConstants.MinLength, MinLength);

            // pattern
            writer.WriteProperty(OpenApiConstants.Pattern, Pattern);

            // maxItems
            writer.WriteProperty(OpenApiConstants.MaxItems, MaxItems);

            // minItems
            writer.WriteProperty(OpenApiConstants.MinItems, MinItems);

            // uniqueItems
            writer.WriteProperty(OpenApiConstants.UniqueItems, UniqueItems);

            // maxProperties
            writer.WriteProperty(OpenApiConstants.MaxProperties, MaxProperties);

            // minProperties
            writer.WriteProperty(OpenApiConstants.MinProperties, MinProperties);

            // required
            writer.WriteOptionalCollection(OpenApiConstants.Required, Required, (w, s) => w.WriteValue(s));

            // enum
            writer.WriteOptionalCollection(OpenApiConstants.Enum, Enum, (w, s) => w.WriteAny(s));

            // type
            writer.WriteProperty(OpenApiConstants.Type, Type);

            // items
            writer.WriteOptionalObject(OpenApiConstants.Items, Items, (w, s) => s.SerializeAsV2(w));

            // allOf
            writer.WriteOptionalCollection(OpenApiConstants.AllOf, AllOf, (w, s) => s.SerializeAsV2(w));

            // properties
            writer.WriteOptionalMap(OpenApiConstants.Properties, Properties, (w, key, s) =>
                s.SerializeAsV2(w, Required, key));

            // additionalProperties
            writer.WriteOptionalObject(
                OpenApiConstants.AdditionalProperties,
                AdditionalProperties,
                (w, s) => s.SerializeAsV2(w));

            // discriminator
            writer.WriteProperty(OpenApiConstants.Discriminator, Discriminator?.PropertyName);

            // readOnly
            // In V2 schema if a property is part of required properties of parent schema,
            // it cannot be marked as readonly.
            if (!parentRequiredProperties.Contains(propertyName))
            {
                writer.WriteProperty(name: OpenApiConstants.ReadOnly, value: ReadOnly, defaultValue: false);
            }

            // xml
            writer.WriteOptionalObject(OpenApiConstants.Xml, Xml, (w, s) => s.SerializeAsV2(w));

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, s) => s.SerializeAsV2(w));

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, e) => w.WriteAny(e));

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);
        }

        internal async Task WriteAsSchemaPropertiesAsync(
            IOpenApiWriter writer,
            ISet<string> parentRequiredProperties,
            string propertyName)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            // format
            await writer.WritePropertyAsync(OpenApiConstants.Format, Format);

            // title
            await writer.WritePropertyAsync(OpenApiConstants.Title, Title);

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);

            // default
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Default, Default, async (w, d) => await w.WriteAnyAsync(d));

            // multipleOf
            await writer.WritePropertyAsync(OpenApiConstants.MultipleOf, MultipleOf);

            // maximum
            await writer.WritePropertyAsync(OpenApiConstants.Maximum, Maximum);

            // exclusiveMaximum
            await writer.WritePropertyAsync(OpenApiConstants.ExclusiveMaximum, ExclusiveMaximum);

            // minimum
            await writer.WritePropertyAsync(OpenApiConstants.Minimum, Minimum);

            // exclusiveMinimum
            await writer.WritePropertyAsync(OpenApiConstants.ExclusiveMinimum, ExclusiveMinimum);

            // maxLength
            await writer.WritePropertyAsync(OpenApiConstants.MaxLength, MaxLength);

            // minLength
            await writer.WritePropertyAsync(OpenApiConstants.MinLength, MinLength);

            // pattern
            await writer.WritePropertyAsync(OpenApiConstants.Pattern, Pattern);

            // maxItems
            await writer.WritePropertyAsync(OpenApiConstants.MaxItems, MaxItems);

            // minItems
            await writer.WritePropertyAsync(OpenApiConstants.MinItems, MinItems);

            // uniqueItems
            await writer.WritePropertyAsync(OpenApiConstants.UniqueItems, UniqueItems);

            // maxProperties
            await writer.WritePropertyAsync(OpenApiConstants.MaxProperties, MaxProperties);

            // minProperties
            await writer.WritePropertyAsync(OpenApiConstants.MinProperties, MinProperties);

            // required
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Required, Required, async (w, s) => await w.WriteValueAsync(s));

            // enum
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Enum, Enum, async (w, s) => await w.WriteAnyAsync(s));

            // type
            await writer.WritePropertyAsync(OpenApiConstants.Type, Type);

            // items
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Items, Items, async (w, s) => await s.SerializeAsV2Async(w));

            // allOf
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.AllOf, AllOf, async (w, s) => await s.SerializeAsV2Async(w));

            // properties
            await writer.WriteOptionalMapAsync(OpenApiConstants.Properties, Properties, async (w, key, s) =>
                await s.SerializeAsV2Async(w, Required, key));

            // additionalProperties
            await writer.WriteOptionalObjectAsync(
                OpenApiConstants.AdditionalProperties,
                AdditionalProperties,
                async (w, s) => await s.SerializeAsV2Async(w));

            // discriminator
            await writer.WritePropertyAsync(OpenApiConstants.Discriminator, Discriminator?.PropertyName);

            // readOnly
            // In V2 schema if a property is part of required properties of parent schema,
            // it cannot be marked as readonly.
            if (!parentRequiredProperties.Contains(propertyName))
            {
                await writer.WritePropertyAsync(name: OpenApiConstants.ReadOnly, value: ReadOnly, defaultValue: false);
            }

            // xml
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Xml, Xml, async (w, s) => await s.SerializeAsV2Async(w));

            // externalDocs
            await writer.WriteOptionalObjectAsync(OpenApiConstants.ExternalDocs, ExternalDocs, async (w, s) => await s.SerializeAsV2Async(w));

            // example
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Example, Example, async (w, e) => await w.WriteAnyAsync(e));

            // extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi2_0);
        }
    }
}