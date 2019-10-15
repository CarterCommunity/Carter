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
    /// Link Object.
    /// </summary>
    public class OpenApiLink : IOpenApiSerializable, IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// A relative or absolute reference to an OAS operation.
        /// This field is mutually exclusive of the operationId field, and MUST point to an Operation Object.
        /// </summary>
        public string OperationRef { get; set; }

        /// <summary>
        /// The name of an existing, resolvable OAS operation, as defined with a unique operationId.
        /// This field is mutually exclusive of the operationRef field.
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        /// A map representing parameters to pass to an operation as specified with operationId or identified via operationRef.
        /// </summary>
        public Dictionary<string, RuntimeExpressionAnyWrapper> Parameters { get; set; } =
            new Dictionary<string, RuntimeExpressionAnyWrapper>();

        /// <summary>
        /// A literal value or {expression} to use as a request body when calling the target operation.
        /// </summary>
        public RuntimeExpressionAnyWrapper RequestBody { get; set; }

        /// <summary>
        /// A description of the link.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A server object to be used by the target operation.
        /// </summary>
        public OpenApiServer Server { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference pointer.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiLink"/> to Open Api v3.0
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
        /// Serialize <see cref="OpenApiLink"/> to Open Api v3.0
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

            // operationRef
            writer.WriteProperty(OpenApiConstants.OperationRef, OperationRef);

            // operationId
            writer.WriteProperty(OpenApiConstants.OperationId, OperationId);

            // parameters
            writer.WriteOptionalMap(OpenApiConstants.Parameters, Parameters, (w, p) => p.WriteValue(w));

            // requestBody
            writer.WriteOptionalObject(OpenApiConstants.RequestBody, RequestBody, (w, r) => r.WriteValue(w));

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // server
            writer.WriteOptionalObject(OpenApiConstants.Server, Server, (w, s) => s.SerializeAsV3(w));

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>
        public async Task SerializeAsV3WithoutReferenceAsync(IOpenApiWriter writer)
        {
            await writer.WriteStartObjectAsync();

            // operationRef
            await writer.WritePropertyAsync(OpenApiConstants.OperationRef, OperationRef);

            // operationId
            await writer.WritePropertyAsync(OpenApiConstants.OperationId, OperationId);

            // parameters
            await writer.WriteOptionalMapAsync(OpenApiConstants.Parameters, Parameters, async (w, p) => await p.WriteValueAsync(w));

            // requestBody
            await writer.WriteOptionalObjectAsync(OpenApiConstants.RequestBody, RequestBody, async (w, r) => await r.WriteValueAsync(w));

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);

            // server
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Server, Server, async (w, s) => await s.SerializeAsV3Async(w));

            await writer.WriteEndObjectAsync();
        }


        /// <summary>
        /// Serialize <see cref="OpenApiLink"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Link object does not exist in V2.
        }

        /// <summary>
        /// Serialize <see cref="OpenApiLink"/> to Open Api v2.0
        /// </summary>
        public Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
        {
            // Link object does not exist in V2.
        }
    }
}