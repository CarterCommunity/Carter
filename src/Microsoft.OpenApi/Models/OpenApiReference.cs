// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Threading.Tasks;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// A simple object to allow referencing other components in the specification, internally and externally.
    /// </summary>
    public class OpenApiReference : IOpenApiSerializable
    {
        /// <summary>
        /// External resource in the reference.
        /// It maybe:
        /// 1. a absolute/relative file path, for example:  ../commons/pet.json
        /// 2. a Url, for example: http://localhost/pet.json
        /// </summary>
        public string ExternalResource { get; set; }

        /// <summary>
        /// The element type referenced.
        /// </summary>
        /// <remarks>This must be present if <see cref="ExternalResource"/> is not present.</remarks>
        public ReferenceType? Type { get; set; }

        /// <summary>
        /// The identifier of the reusable component of one particular ReferenceType.
        /// If ExternalResource is present, this is the path to the component after the '#/'.
        /// For example, if the reference is 'example.json#/path/to/component', the Id is 'path/to/component'.
        /// If ExternalResource is not present, this is the name of the component without the reference type name.
        /// For example, if the reference is '#/components/schemas/componentName', the Id is 'componentName'.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets a flag indicating whether this reference is an external reference.
        /// </summary>
        public bool IsExternal => ExternalResource != null;

        /// <summary>
        /// Gets a flag indicating whether this reference is a local reference.
        /// </summary>
        public bool IsLocal => ExternalResource == null;

        /// <summary>
        /// Gets the full reference string for v3.0.
        /// </summary>
        public string ReferenceV3
        {
            get
            {
                if (IsExternal)
                {
                    return GetExternalReference();
                }

                if (!Type.HasValue)
                {
                    throw Error.ArgumentNull(nameof(Type));
                }

                if (Type == ReferenceType.Tag)
                {
                    return Id;
                }

                if (Type == ReferenceType.SecurityScheme)
                {
                    return Id;
                }

                return "#/components/" + Type.GetDisplayName() + "/" + Id;
            }
        }

        /// <summary>
        /// Gets the full reference string for V2.0
        /// </summary>
        public string ReferenceV2
        {
            get
            {
                if (IsExternal)
                {
                    return GetExternalReference();
                }

                if (!Type.HasValue)
                {
                    throw Error.ArgumentNull(nameof(Type));
                }

                if (Type == ReferenceType.Tag)
                {
                    return Id;
                }

                if (Type == ReferenceType.SecurityScheme)
                {
                    return Id;
                }

                return "#/" + GetReferenceTypeNameAsV2(Type.Value) + "/" + Id;
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiReference"/> to Open Api v3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (Type == ReferenceType.Tag)
            {
                // Write the string value only
                writer.WriteValue(ReferenceV3);
                return;
            }

            if (Type == ReferenceType.SecurityScheme)
            {
                // Write the string as property name
                writer.WritePropertyName(ReferenceV3);
                return;
            }

            writer.WriteStartObject();

            // $ref
            writer.WriteProperty(OpenApiConstants.DollarRef, ReferenceV3);

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiReference"/> to Open Api v3.0.
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (Type == ReferenceType.Tag)
            {
                // Write the string value only
                await writer.WriteValueAsync(ReferenceV3);
                return;
            }

            if (Type == ReferenceType.SecurityScheme)
            {
                // Write the string as property name
                await writer.WritePropertyNameAsync(ReferenceV3);
                return;
            }

            await writer.WriteStartObjectAsync();

            // $ref
            await writer.WritePropertyAsync(OpenApiConstants.DollarRef, ReferenceV3);

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiReference"/> to Open Api v2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (Type == ReferenceType.Tag)
            {
                // Write the string value only
                writer.WriteValue(ReferenceV2);
                return;
            }

            if (Type == ReferenceType.SecurityScheme)
            {
                // Write the string as property name
                writer.WritePropertyName(ReferenceV2);
                return;
            }

            writer.WriteStartObject();

            // $ref
            writer.WriteProperty(OpenApiConstants.DollarRef, ReferenceV2);

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiReference"/> to Open Api v2.0.
        /// </summary>
        public async Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (Type == ReferenceType.Tag)
            {
                // Write the string value only
                await writer.WriteValueAsync(ReferenceV2);
                return;
            }

            if (Type == ReferenceType.SecurityScheme)
            {
                // Write the string as property name
                await writer.WritePropertyNameAsync(ReferenceV2);
                return;
            }

            await writer.WriteStartObjectAsync();

            // $ref
            await writer.WritePropertyAsync(OpenApiConstants.DollarRef, ReferenceV2);

            await writer.WriteEndObjectAsync();
        }

        private string GetExternalReference()
        {
            if (Id != null)
            {
                return ExternalResource + "#/" + Id;
            }

            return ExternalResource;
        }

        private string GetReferenceTypeNameAsV2(ReferenceType type)
        {
            switch (type)
            {
                case ReferenceType.Schema:
                    return OpenApiConstants.Definitions;

                case ReferenceType.Parameter:
                    return OpenApiConstants.Parameters;

                case ReferenceType.Response:
                    return OpenApiConstants.Responses;

                case ReferenceType.Header:
                    return OpenApiConstants.Headers;

                case ReferenceType.Tag:
                    return OpenApiConstants.Tags;

                case ReferenceType.SecurityScheme:
                    return OpenApiConstants.SecurityDefinitions;

                default:
                    // If the reference type is not supported in V2, simply return null
                    // to indicate that the reference is not pointing to any object.
                    return null;
            }
        }
    }
}