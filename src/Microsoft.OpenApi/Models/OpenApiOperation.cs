// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Operation Object.
    /// </summary>
    public class OpenApiOperation : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// Default value for <see cref="Deprecated"/>.
        /// </summary>
        public const bool DeprecatedDefault = false;

        /// <summary>
        /// A list of tags for API documentation control.
        /// Tags can be used for logical grouping of operations by resources or any other qualifier.
        /// </summary>
        public IList<OpenApiTag> Tags { get; set; } = new List<OpenApiTag>();

        /// <summary>
        /// A short summary of what the operation does.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// A verbose explanation of the operation behavior.
        /// CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Additional external documentation for this operation.
        /// </summary>
        public OpenApiExternalDocs ExternalDocs { get; set; }

        /// <summary>
        /// Unique string used to identify the operation. The id MUST be unique among all operations described in the API.
        /// Tools and libraries MAY use the operationId to uniquely identify an operation, therefore,
        /// it is RECOMMENDED to follow common programming naming conventions.
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        /// A list of parameters that are applicable for this operation.
        /// If a parameter is already defined at the Path Item, the new definition will override it but can never remove it.
        /// The list MUST NOT include duplicated parameters. A unique parameter is defined by a combination of a name and location.
        /// The list can use the Reference Object to link to parameters that are defined at the OpenAPI Object's components/parameters.
        /// </summary>
        public IList<OpenApiParameter> Parameters { get; set; } = new List<OpenApiParameter>();

        /// <summary>
        /// The request body applicable for this operation.
        /// The requestBody is only supported in HTTP methods where the HTTP 1.1 specification RFC7231
        /// has explicitly defined semantics for request bodies.
        /// In other cases where the HTTP spec is vague, requestBody SHALL be ignored by consumers.
        /// </summary>
        public OpenApiRequestBody RequestBody { get; set; }

        /// <summary>
        /// REQUIRED. The list of possible responses as they are returned from executing this operation.
        /// </summary>
        public OpenApiResponses Responses { get; set; } = new OpenApiResponses();

        /// <summary>
        /// A map of possible out-of band callbacks related to the parent operation.
        /// The key is a unique identifier for the Callback Object.
        /// Each value in the map is a Callback Object that describes a request
        /// that may be initiated by the API provider and the expected responses.
        /// The key value used to identify the callback object is an expression, evaluated at runtime,
        /// that identifies a URL to use for the callback operation.
        /// </summary>
        public IDictionary<string, OpenApiCallback> Callbacks { get; set; } = new Dictionary<string, OpenApiCallback>();

        /// <summary>
        /// Declares this operation to be deprecated. Consumers SHOULD refrain from usage of the declared operation.
        /// </summary>
        public bool Deprecated { get; set; } = DeprecatedDefault;

        /// <summary>
        /// A declaration of which security mechanisms can be used for this operation.
        /// The list of values includes alternative security requirement objects that can be used.
        /// Only one of the security requirement objects need to be satisfied to authorize a request.
        /// This definition overrides any declared top-level security.
        /// To remove a top-level security declaration, an empty array can be used.
        /// </summary>
        public IList<OpenApiSecurityRequirement> Security { get; set; } = new List<OpenApiSecurityRequirement>();

        /// <summary>
        /// An alternative server array to service this operation.
        /// If an alternative server object is specified at the Path Item Object or Root level,
        /// it will be overridden by this value.
        /// </summary>
        public IList<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize <see cref="OpenApiOperation"/> to Open Api v3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // tags
            writer.WriteOptionalCollection(
                OpenApiConstants.Tags,
                Tags,
                (w, t) => { t.SerializeAsV3(w); });

            // summary
            writer.WriteProperty(OpenApiConstants.Summary, Summary);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, e) => e.SerializeAsV3(w));

            // operationId
            writer.WriteProperty(OpenApiConstants.OperationId, OperationId);

            // parameters
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, Parameters, (w, p) => p.SerializeAsV3(w));

            // requestBody
            writer.WriteOptionalObject(OpenApiConstants.RequestBody, RequestBody, (w, r) => r.SerializeAsV3(w));

            // responses
            writer.WriteRequiredObject(OpenApiConstants.Responses, Responses, (w, r) => r.SerializeAsV3(w));

            // callbacks
            writer.WriteOptionalMap(OpenApiConstants.Callbacks, Callbacks, (w, c) => c.SerializeAsV3(w));

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);

            // security
            writer.WriteOptionalCollection(OpenApiConstants.Security, Security, (w, s) => s.SerializeAsV3(w));

            // servers
            writer.WriteOptionalCollection(OpenApiConstants.Servers, Servers, (w, s) => s.SerializeAsV3(w));

            // specification extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOperation"/> to Open Api v3.0.
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // tags
            await writer.WriteOptionalCollectionAsync(
                OpenApiConstants.Tags,
                Tags,
                async (w, t) => { await t.SerializeAsV3Async(w); });

            // summary
            await writer.WritePropertyAsync(OpenApiConstants.Summary, Summary);

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);

            // externalDocs
            await writer.WriteOptionalObjectAsync(OpenApiConstants.ExternalDocs, ExternalDocs, async (w, e) => await e.SerializeAsV3Async(w));

            // operationId
            await writer.WritePropertyAsync(OpenApiConstants.OperationId, OperationId);

            // parameters
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Parameters, Parameters, async (w, p) => await p.SerializeAsV3Async(w));

            // requestBody
            await writer.WriteOptionalObjectAsync(OpenApiConstants.RequestBody, RequestBody, async (w, r) => await r.SerializeAsV3Async(w));

            // responses
            await writer.WriteRequiredObjectAsync(OpenApiConstants.Responses, Responses, async (w, r) => await r.SerializeAsV3Async(w));

            // callbacks
            await writer.WriteOptionalMapAsync(OpenApiConstants.Callbacks, Callbacks, async (w, c) => await c.SerializeAsV3Async(w));

            // deprecated
            await writer.WritePropertyAsync(OpenApiConstants.Deprecated, Deprecated, false);

            // security
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Security, Security, async (w, s) => await s.SerializeAsV3Async(w));

            // servers
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Servers, Servers, async (w, s) => await s.SerializeAsV3Async(w));

            // specification extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOperation"/> to Open Api v2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // tags
            writer.WriteOptionalCollection(
                OpenApiConstants.Tags,
                Tags,
                (w, t) => { t.SerializeAsV2(w); });

            // summary
            writer.WriteProperty(OpenApiConstants.Summary, Summary);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, e) => e.SerializeAsV2(w));

            // operationId
            writer.WriteProperty(OpenApiConstants.OperationId, OperationId);

            IList<OpenApiParameter> parameters;
            if (Parameters == null)
            {
                parameters = new List<OpenApiParameter>();
            }
            else
            {
                parameters = new List<OpenApiParameter>(Parameters);
            }

            if (RequestBody != null)
            {
                // consumes
                writer.WritePropertyName(OpenApiConstants.Consumes);
                writer.WriteStartArray();
                var consumes = RequestBody.Content.Keys.Distinct().ToList();
                foreach (var mediaType in consumes)
                {
                    writer.WriteValue(mediaType);
                }

                writer.WriteEndArray();

                // This is form data. We need to split the request body into multiple parameters.
                if (consumes.Contains("application/x-www-form-urlencoded") ||
                    consumes.Contains("multipart/form-data"))
                {
                    foreach (var property in RequestBody.Content.First().Value.Schema.Properties)
                    {
                        parameters.Add(
                            new OpenApiFormDataParameter
                            {
                                Description = property.Value.Description,
                                Name = property.Key,
                                Schema = property.Value,
                                Required = RequestBody.Content.First().Value.Schema.Required.Contains(property.Key)
                            });
                    }
                }
                else
                {
                    var content = RequestBody.Content.Values.FirstOrDefault();
                    var bodyParameter = new OpenApiBodyParameter
                    {
                        Description = RequestBody.Description,
                        // V2 spec actually allows the body to have custom name.
                        // Our library does not support this at the moment.
                        Name = "body",
                        Schema = content?.Schema ?? new OpenApiSchema(),
                        Required = RequestBody.Required
                    };

                    parameters.Add(bodyParameter);
                }
            }

            if (Responses != null)
            {
                var produces = Responses.Where(r => r.Value.Content != null)
                    .SelectMany(r => r.Value.Content?.Keys)
                    .Distinct()
                    .ToList();

                if (produces.Any())
                {
                    // produces
                    writer.WritePropertyName(OpenApiConstants.Produces);
                    writer.WriteStartArray();
                    foreach (var mediaType in produces)
                    {
                        writer.WriteValue(mediaType);
                    }

                    writer.WriteEndArray();
                }
            }

            // parameters
            // Use the parameters created locally to include request body if exists.
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, parameters, (w, p) => p.SerializeAsV2(w));

            // responses
            writer.WriteRequiredObject(OpenApiConstants.Responses, Responses, (w, r) => r.SerializeAsV2(w));

            // schemes
            // All schemes in the Servers are extracted, regardless of whether the host matches
            // the host defined in the outermost Swagger object. This is due to the 
            // inaccessibility of information for that host in the context of an inner object like this Operation.
            if (Servers != null)
            {
                var schemes = Servers.Select(
                        s =>
                        {
                            Uri.TryCreate(s.Url, UriKind.RelativeOrAbsolute, out var url);
                            return url?.Scheme;
                        })
                    .Where(s => s != null)
                    .Distinct()
                    .ToList();

                writer.WriteOptionalCollection(OpenApiConstants.Schemes, schemes, (w, s) => w.WriteValue(s));
            }

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);

            // security
            writer.WriteOptionalCollection(OpenApiConstants.Security, Security, (w, s) => s.SerializeAsV2(w));

            // specification extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }


        /// <summary>
        /// Serialize <see cref="OpenApiOperation"/> to Open Api v2.0.
        /// </summary>
        public async Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // tags
            await writer.WriteOptionalCollectionAsync(
                OpenApiConstants.Tags,
                Tags,
                async (w, t) => { await t.SerializeAsV2Async(w); });

            // summary
            await writer.WritePropertyAsync(OpenApiConstants.Summary, Summary);

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);

            // externalDocs
            await writer.WriteOptionalObjectAsync(OpenApiConstants.ExternalDocs, ExternalDocs, async (w, e) => await e.SerializeAsV2Async(w));

            // operationId
            await writer.WritePropertyAsync(OpenApiConstants.OperationId, OperationId);

            IList<OpenApiParameter> parameters;
            if (Parameters == null)
            {
                parameters = new List<OpenApiParameter>();
            }
            else
            {
                parameters = new List<OpenApiParameter>(Parameters);
            }

            if (RequestBody != null)
            {
                // consumes
                await writer.WritePropertyNameAsync(OpenApiConstants.Consumes);
                await writer.WriteStartArrayAsync();
                var consumes = RequestBody.Content.Keys.Distinct().ToList();
                foreach (var mediaType in consumes)
                {
                    await writer.WriteValueAsync(mediaType);
                }

                await writer.WriteEndArrayAsync();

                // This is form data. We need to split the request body into multiple parameters.
                if (consumes.Contains("application/x-www-form-urlencoded") ||
                    consumes.Contains("multipart/form-data"))
                {
                    foreach (var property in RequestBody.Content.First().Value.Schema.Properties)
                    {
                        parameters.Add(
                            new OpenApiFormDataParameter
                            {
                                Description = property.Value.Description,
                                Name = property.Key,
                                Schema = property.Value,
                                Required = RequestBody.Content.First().Value.Schema.Required.Contains(property.Key)
                            });
                    }
                }
                else
                {
                    var content = RequestBody.Content.Values.FirstOrDefault();
                    var bodyParameter = new OpenApiBodyParameter
                    {
                        Description = RequestBody.Description,
                        // V2 spec actually allows the body to have custom name.
                        // Our library does not support this at the moment.
                        Name = "body",
                        Schema = content?.Schema ?? new OpenApiSchema(),
                        Required = RequestBody.Required
                    };

                    parameters.Add(bodyParameter);
                }
            }

            if (Responses != null)
            {
                var produces = Responses.Where(r => r.Value.Content != null)
                    .SelectMany(r => r.Value.Content?.Keys)
                    .Distinct()
                    .ToList();

                if (produces.Any())
                {
                    // produces
                    await writer.WritePropertyNameAsync(OpenApiConstants.Produces);
                    await writer.WriteStartArrayAsync();
                    foreach (var mediaType in produces)
                    {
                        await writer.WriteValueAsync(mediaType);
                    }

                    await writer.WriteEndArrayAsync();
                }
            }

            // parameters
            // Use the parameters created locally to include request body if exists.
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Parameters, parameters, async (w, p) => await p.SerializeAsV2Async(w));

            // responses
            await writer.WriteRequiredObjectAsync(OpenApiConstants.Responses, Responses, async (w, r) => await r.SerializeAsV2Async(w));

            // schemes
            // All schemes in the Servers are extracted, regardless of whether the host matches
            // the host defined in the outermost Swagger object. This is due to the 
            // inaccessibility of information for that host in the context of an inner object like this Operation.
            if (Servers != null)
            {
                var schemes = Servers.Select(
                        s =>
                        {
                            Uri.TryCreate(s.Url, UriKind.RelativeOrAbsolute, out var url);
                            return url?.Scheme;
                        })
                    .Where(s => s != null)
                    .Distinct()
                    .ToList();

                await writer.WriteOptionalCollectionAsync(OpenApiConstants.Schemes, schemes, async (w, s) => await w.WriteValueAsync(s));
            }

            // deprecated
            await writer.WritePropertyAsync(OpenApiConstants.Deprecated, Deprecated, false);

            // security
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Security, Security, async (w, s) => await s.SerializeAsV2Async(w));

            // specification extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi2_0);

            await writer.WriteEndObjectAsync();
        }
    }
}