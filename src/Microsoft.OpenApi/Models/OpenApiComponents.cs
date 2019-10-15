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
    /// Components Object.
    /// </summary>
    public class OpenApiComponents : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// An object to hold reusable <see cref="OpenApiSchema"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiSchema> Schemas { get; set; } = new Dictionary<string, OpenApiSchema>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiResponse"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiResponse> Responses { get; set; } = new Dictionary<string, OpenApiResponse>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiParameter"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiParameter> Parameters { get; set; } =
            new Dictionary<string, OpenApiParameter>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiExample"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiExample> Examples { get; set; } = new Dictionary<string, OpenApiExample>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiRequestBody"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiRequestBody> RequestBodies { get; set; } =
            new Dictionary<string, OpenApiRequestBody>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiHeader"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiHeader> Headers { get; set; } = new Dictionary<string, OpenApiHeader>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiSecurityScheme"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiSecurityScheme> SecuritySchemes { get; set; } =
            new Dictionary<string, OpenApiSecurityScheme>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiLink"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiLink> Links { get; set; } = new Dictionary<string, OpenApiLink>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiCallback"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiCallback> Callbacks { get; set; } = new Dictionary<string, OpenApiCallback>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open Api v3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // Serialize each referenceable object as full object without reference if the reference in the object points to itself.
            // If the reference exists but points to other objects, the object is serialized to just that reference.

            // schemas
            writer.WriteOptionalMap(
                OpenApiConstants.Schemas,
                Schemas,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Schema &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // responses
            writer.WriteOptionalMap(
                OpenApiConstants.Responses,
                Responses,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Response &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // parameters
            writer.WriteOptionalMap(
                OpenApiConstants.Parameters,
                Parameters,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Parameter &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // examples
            writer.WriteOptionalMap(
                OpenApiConstants.Examples,
                Examples,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Example &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // requestBodies
            writer.WriteOptionalMap(
                OpenApiConstants.RequestBodies,
                RequestBodies,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.RequestBody &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // headers
            writer.WriteOptionalMap(
                OpenApiConstants.Headers,
                Headers,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Header &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // securitySchemes
            writer.WriteOptionalMap(
                OpenApiConstants.SecuritySchemes,
                SecuritySchemes,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.SecurityScheme &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // links
            writer.WriteOptionalMap(
                OpenApiConstants.Links,
                Links,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Link &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // callbacks
            writer.WriteOptionalMap(
                OpenApiConstants.Callbacks,
                Callbacks,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Callback &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open Api v3.0.
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // Serialize each referenceable object as full object without reference if the reference in the object points to itself.
            // If the reference exists but points to other objects, the object is serialized to just that reference.

            // schemas
            await writer.WriteOptionalMapAsync(
                OpenApiConstants.Schemas,
                Schemas,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Schema &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // responses
            writer.WriteOptionalMap(
                OpenApiConstants.Responses,
                Responses,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Response &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // parameters
            writer.WriteOptionalMap(
                OpenApiConstants.Parameters,
                Parameters,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Parameter &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // examples
            writer.WriteOptionalMap(
                OpenApiConstants.Examples,
                Examples,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Example &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // requestBodies
            writer.WriteOptionalMap(
                OpenApiConstants.RequestBodies,
                RequestBodies,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.RequestBody &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // headers
            writer.WriteOptionalMap(
                OpenApiConstants.Headers,
                Headers,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Header &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // securitySchemes
            writer.WriteOptionalMap(
                OpenApiConstants.SecuritySchemes,
                SecuritySchemes,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.SecurityScheme &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // links
            writer.WriteOptionalMap(
                OpenApiConstants.Links,
                Links,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Link &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // callbacks
            writer.WriteOptionalMap(
                OpenApiConstants.Callbacks,
                Callbacks,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Callback &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }


        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open Api v2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Components object does not exist in V2.
        }

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open Api v2.0.
        /// </summary>
        public Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            // Components object does not exist in V2.
            return Task.CompletedTask;
        }
    }
}