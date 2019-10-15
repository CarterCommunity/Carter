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
    /// Path Item Object: to describe the operations available on a single path.
    /// </summary>
    public class OpenApiPathItem : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// An optional, string summary, intended to apply to all operations in this path.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// An optional, string description, intended to apply to all operations in this path.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the definition of operations on this path.
        /// </summary>
        public IDictionary<OperationType, OpenApiOperation> Operations { get; set; }
            = new Dictionary<OperationType, OpenApiOperation>();

        /// <summary>
        /// An alternative server array to service all operations in this path.
        /// </summary>
        public IList<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();

        /// <summary>
        /// A list of parameters that are applicable for all the operations described under this path.
        /// These parameters can be overridden at the operation level, but cannot be removed there.
        /// </summary>
        public IList<OpenApiParameter> Parameters { get; set; } = new List<OpenApiParameter>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Add one operation into this path item.
        /// </summary>
        /// <param name="operationType">The operation type kind.</param>
        /// <param name="operation">The operation item.</param>
        public void AddOperation(OperationType operationType, OpenApiOperation operation)
        {
            Operations[operationType] = operation;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // summary
            writer.WriteProperty(OpenApiConstants.Summary, Summary);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // operations
            foreach (var operation in Operations)
            {
                writer.WriteOptionalObject(
                    operation.Key.GetDisplayName(),
                    operation.Value,
                    (w, o) => o.SerializeAsV3(w));
            }

            // servers
            writer.WriteOptionalCollection(OpenApiConstants.Servers, Servers, (w, s) => s.SerializeAsV3(w));

            // parameters
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, Parameters, (w, p) => p.SerializeAsV3(w));

            // specification extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.0
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // summary
            await writer.WritePropertyAsync(OpenApiConstants.Summary, Summary);

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);

            // operations
            foreach (var operation in Operations)
            {
                await writer.WriteOptionalObjectAsync(
                    operation.Key.GetDisplayName(),
                    operation.Value, async (w, o) => await o.SerializeAsV3Async(w));
            }

            // servers
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Servers, Servers, async (w, s) => await s.SerializeAsV3Async(w));

            // parameters
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Parameters, Parameters, async (w, p) => await p.SerializeAsV3Async(w));

            // specification extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // operations except "trace"
            foreach (var operation in Operations)
            {
                if (operation.Key != OperationType.Trace)
                {
                    writer.WriteOptionalObject(
                        operation.Key.GetDisplayName(),
                        operation.Value,
                        (w, o) => o.SerializeAsV2(w));
                }
            }

            // parameters
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, Parameters, (w, p) => p.SerializeAsV2(w));

            // write "summary" as extensions
            writer.WriteProperty(OpenApiConstants.ExtensionFieldNamePrefix + OpenApiConstants.Summary, Summary);

            // write "description" as extensions
            writer.WriteProperty(
                OpenApiConstants.ExtensionFieldNamePrefix + OpenApiConstants.Description,
                Description);

            // specification extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v2.0
        /// </summary>
        public async Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // operations except "trace"
            foreach (var operation in Operations)
            {
                if (operation.Key != OperationType.Trace)
                {
                    await writer.WriteOptionalObjectAsync(
                        operation.Key.GetDisplayName(),
                        operation.Value,
                        async (w, o) => await o.SerializeAsV2Async(w));
                }
            }

            // parameters
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Parameters, Parameters, async (w, p) => await p.SerializeAsV2Async(w));

            // write "summary" as extensions
            await writer.WritePropertyAsync(OpenApiConstants.ExtensionFieldNamePrefix + OpenApiConstants.Summary, Summary);

            // write "description" as extensions
            await writer.WritePropertyAsync(
                OpenApiConstants.ExtensionFieldNamePrefix + OpenApiConstants.Description,
                Description);

            // specification extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi2_0);

            await writer.WriteEndObjectAsync();
        }
    }
}