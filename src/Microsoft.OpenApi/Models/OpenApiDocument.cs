// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Describes an OpenAPI object (OpenAPI document). See: https://swagger.io/specification
    /// </summary>
    public class OpenApiDocument : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. Provides metadata about the API. The metadata MAY be used by tooling as required.
        /// </summary>
        public OpenApiInfo Info { get; set; }

        /// <summary>
        /// An array of Server Objects, which provide connectivity information to a target server.
        /// </summary>
        public IList<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();

        /// <summary>
        /// REQUIRED. The available paths and operations for the API.
        /// </summary>
        public OpenApiPaths Paths { get; set; }

        /// <summary>
        /// An element to hold various schemas for the specification.
        /// </summary>
        public OpenApiComponents Components { get; set; }

        /// <summary>
        /// A declaration of which security mechanisms can be used across the API.
        /// </summary>
        public IList<OpenApiSecurityRequirement> SecurityRequirements { get; set; } =
            new List<OpenApiSecurityRequirement>();

        /// <summary>
        /// A list of tags used by the specification with additional metadata.
        /// </summary>
        public IList<OpenApiTag> Tags { get; set; } = new List<OpenApiTag>();

        /// <summary>
        /// Additional external documentation.
        /// </summary>
        public OpenApiExternalDocs ExternalDocs { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to the latest patch of OpenAPI object V3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // openapi
            writer.WriteProperty(OpenApiConstants.OpenApi, "3.0.1");

            // info
            writer.WriteRequiredObject(OpenApiConstants.Info, Info, (w, i) => i.SerializeAsV3(w));

            // servers
            writer.WriteOptionalCollection(OpenApiConstants.Servers, Servers, (w, s) => s.SerializeAsV3(w));

            // paths
            writer.WriteRequiredObject(OpenApiConstants.Paths, Paths, (w, p) => p.SerializeAsV3(w));

            // components
            writer.WriteOptionalObject(OpenApiConstants.Components, Components, (w, c) => c.SerializeAsV3(w));

            // security
            writer.WriteOptionalCollection(
                OpenApiConstants.Security,
                SecurityRequirements,
                (w, s) => s.SerializeAsV3(w));

            // tags
            writer.WriteOptionalCollection(OpenApiConstants.Tags, Tags, (w, t) => t.SerializeAsV3WithoutReference(w));

            // external docs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, e) => e.SerializeAsV3(w));

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to the latest patch of OpenAPI object V3.0.
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // openapi
            await writer.WritePropertyAsync(OpenApiConstants.OpenApi, "3.0.1");

            // info
            await writer.WriteRequiredObjectAsync(OpenApiConstants.Info, Info, async (w, i) => await i.SerializeAsV3Async(w));

            // servers
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Servers, Servers, async (w, s) => await s.SerializeAsV3Async(w));

            // paths
            await writer.WriteRequiredObjectAsync(OpenApiConstants.Paths, Paths, async (w, p) => await p.SerializeAsV3Async(w));

            // components
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Components, Components, async (w, c) => await c.SerializeAsV3Async(w));

            // security
            await writer.WriteOptionalCollectionAsync(
                OpenApiConstants.Security,
                SecurityRequirements,
                async (w, s) => await s.SerializeAsV3Async(w));

            // tags
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Tags, Tags, async (w, t) => await t.SerializeAsV3WithoutReferenceAsync(w));

            // external docs
            await writer.WriteOptionalObjectAsync(OpenApiConstants.ExternalDocs, ExternalDocs, async (w, e) => await e.SerializeAsV3Async(w));

            // extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to OpenAPI object V2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // swagger
            writer.WriteProperty(OpenApiConstants.Swagger, "2.0");

            // info
            writer.WriteRequiredObject(OpenApiConstants.Info, Info, (w, i) => i.SerializeAsV2(w));

            // host, basePath, schemes, consumes, produces
            WriteHostInfoV2(writer, Servers);

            // paths
            writer.WriteRequiredObject(OpenApiConstants.Paths, Paths, (w, p) => p.SerializeAsV2(w));

            // Serialize each referenceable object as full object without reference if the reference in the object points to itself. 
            // If the reference exists but points to other objects, the object is serialized to just that reference.

            // definitions
            writer.WriteOptionalMap(
                OpenApiConstants.Definitions,
                Components?.Schemas,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Schema &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV2WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV2(w);
                    }
                });

            // parameters
            writer.WriteOptionalMap(
                OpenApiConstants.Parameters,
                Components?.Parameters,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Parameter &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV2WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV2(w);
                    }
                });

            // responses
            writer.WriteOptionalMap(
                OpenApiConstants.Responses,
                Components?.Responses,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Response &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV2WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV2(w);
                    }
                });

            // securityDefinitions
            writer.WriteOptionalMap(
                OpenApiConstants.SecurityDefinitions,
                Components?.SecuritySchemes,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.SecurityScheme &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV2WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV2(w);
                    }
                });

            // security
            writer.WriteOptionalCollection(
                OpenApiConstants.Security,
                SecurityRequirements,
                (w, s) => s.SerializeAsV2(w));

            // tags
            writer.WriteOptionalCollection(OpenApiConstants.Tags, Tags, (w, t) => t.SerializeAsV2WithoutReference(w));

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, e) => e.SerializeAsV2(w));

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to OpenAPI object V2.0.
        /// </summary>
        public async Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // swagger
            await writer.WritePropertyAsync(OpenApiConstants.Swagger, "2.0");

            // info
            await writer.WriteRequiredObjectAsync(OpenApiConstants.Info, Info, async (w, i) => await i.SerializeAsV2Async(w));

            // host, basePath, schemes, consumes, produces
            await WriteHostInfoV2Async(writer, Servers);

            // paths
            await writer.WriteRequiredObjectAsync(OpenApiConstants.Paths, Paths, async (w, p) => await p.SerializeAsV2Async(w));

            // Serialize each referenceable object as full object without reference if the reference in the object points to itself. 
            // If the reference exists but points to other objects, the object is serialized to just that reference.

            // definitions
            await writer.WriteOptionalMapAsync(
                OpenApiConstants.Definitions,
                Components?.Schemas,
                async (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Schema &&
                        component.Reference.Id == key)
                    {
                        await component.SerializeAsV2WithoutReferenceAsync(w);
                    }
                    else
                    {
                        await component.SerializeAsV2Async(w);
                    }
                });

            // parameters
            await writer.WriteOptionalMapAsync(
                OpenApiConstants.Parameters,
                Components?.Parameters,
                async (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Parameter &&
                        component.Reference.Id == key)
                    {
                        await component.SerializeAsV2WithoutReferenceAsync(w);
                    }
                    else
                    {
                        await component.SerializeAsV2Async(w);
                    }
                });

            // responses
            await writer.WriteOptionalMapAsync(
                OpenApiConstants.Responses,
                Components?.Responses,
                async (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Response &&
                        component.Reference.Id == key)
                    {
                        await component.SerializeAsV2WithoutReferenceAsync(w);
                    }
                    else
                    {
                        await component.SerializeAsV2Async(w);
                    }
                });

            // securityDefinitions
            await writer.WriteOptionalMapAsync(
                OpenApiConstants.SecurityDefinitions,
                Components?.SecuritySchemes,
                async (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.SecurityScheme &&
                        component.Reference.Id == key)
                    {
                        await component.SerializeAsV2WithoutReferenceAsync(w);
                    }
                    else
                    {
                        await component.SerializeAsV2Async(w);
                    }
                });

            // security
            await writer.WriteOptionalCollectionAsync(
                OpenApiConstants.Security,
                SecurityRequirements,
                async (w, s) => await s.SerializeAsV2Async(w));

            // tags
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Tags, Tags, async (w, t) => await t.SerializeAsV2WithoutReferenceAsync(w));

            // externalDocs
            await writer.WriteOptionalObjectAsync(OpenApiConstants.ExternalDocs, ExternalDocs, async (w, e) => await e.SerializeAsV2Async(w));

            // extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi2_0);

            await writer.WriteEndObjectAsync();
        }

        private static void WriteHostInfoV2(IOpenApiWriter writer, IList<OpenApiServer> servers)
        {
            if (servers == null || !servers.Any())
            {
                return;
            }

            // Arbitrarily choose the first server given that V2 only allows 
            // one host, port, and base path.
            var firstServer = servers.First();

            // Divide the URL in the Url property into host and basePath required in OpenAPI V2
            // The Url property cannotcontain path templating to be valid for V2 serialization.
            var firstServerUrl = new Uri(firstServer.Url);

            // host
            writer.WriteProperty(
                OpenApiConstants.Host,
                firstServerUrl.GetComponents(UriComponents.Host | UriComponents.Port, UriFormat.SafeUnescaped));

            // basePath
            if (firstServerUrl.AbsolutePath != "/")
            {
                writer.WriteProperty(OpenApiConstants.BasePath, firstServerUrl.AbsolutePath);
            }

            // Consider all schemes of the URLs in the server list that have the same
            // host, port, and base path as the first server.
            var schemes = servers.Select(
                    s =>
                    {
                        Uri.TryCreate(s.Url, UriKind.RelativeOrAbsolute, out var url);
                        return url;
                    })
                .Where(
                    u => Uri.Compare(
                            u,
                            firstServerUrl,
                            UriComponents.Host | UriComponents.Port | UriComponents.Path,
                            UriFormat.SafeUnescaped,
                            StringComparison.OrdinalIgnoreCase) ==
                        0)
                .Select(u => u.Scheme)
                .Distinct()
                .ToList();

            // schemes
            writer.WriteOptionalCollection(OpenApiConstants.Schemes, schemes, (w, s) => w.WriteValue(s));
        }
        
        private static async Task WriteHostInfoV2Async(IOpenApiWriter writer, IList<OpenApiServer> servers)
        {
            if (servers == null || !servers.Any())
            {
                return;
            }

            // Arbitrarily choose the first server given that V2 only allows 
            // one host, port, and base path.
            var firstServer = servers.First();

            // Divide the URL in the Url property into host and basePath required in OpenAPI V2
            // The Url property cannotcontain path templating to be valid for V2 serialization.
            var firstServerUrl = new Uri(firstServer.Url);

            // host
            await writer.WritePropertyAsync(
                OpenApiConstants.Host,
                firstServerUrl.GetComponents(UriComponents.Host | UriComponents.Port, UriFormat.SafeUnescaped));

            // basePath
            if (firstServerUrl.AbsolutePath != "/")
            {
                await writer.WritePropertyAsync(OpenApiConstants.BasePath, firstServerUrl.AbsolutePath);
            }

            // Consider all schemes of the URLs in the server list that have the same
            // host, port, and base path as the first server.
            var schemes = servers.Select(
                    s =>
                    {
                        Uri.TryCreate(s.Url, UriKind.RelativeOrAbsolute, out var url);
                        return url;
                    })
                .Where(
                    u => Uri.Compare(
                            u,
                            firstServerUrl,
                            UriComponents.Host | UriComponents.Port | UriComponents.Path,
                            UriFormat.SafeUnescaped,
                            StringComparison.OrdinalIgnoreCase) ==
                        0)
                .Select(u => u.Scheme)
                .Distinct()
                .ToList();

            // schemes
            await writer.WriteOptionalCollectionAsync(OpenApiConstants.Schemes, schemes, async (w, s) => await w.WriteValueAsync(s));
        }

        /// <summary>
        /// Load the referenced <see cref="IOpenApiReferenceable"/> object from a <see cref="OpenApiReference"/> object
        /// </summary>
        public IOpenApiReferenceable ResolveReference(OpenApiReference reference)
        {
            if (reference == null)
            {
                return null;
            }

            if (reference.IsExternal)
            {
                // Should not attempt to resolve external references against a single document.
                throw new ArgumentException(Properties.SRResource.RemoteReferenceNotSupported); 
            }

            if (!reference.Type.HasValue)
            {
                throw new ArgumentException(Properties.SRResource.LocalReferenceRequiresType);
            }

            // Special case for Tag
            if (reference.Type == ReferenceType.Tag)
            {
                foreach (var tag in this.Tags)
                {
                    if (tag.Name == reference.Id)
                    {
                        tag.Reference = reference;
                        return tag;
                    }
                }

                return null;
            }

            if (this.Components == null) {
                throw new OpenApiException(string.Format(Properties.SRResource.InvalidReferenceId, reference.Id));
            }

            try
            {
                switch (reference.Type)
                {
                    case ReferenceType.Schema:
                        return this.Components.Schemas[reference.Id];

                    case ReferenceType.Response:
                        return this.Components.Responses[reference.Id];

                    case ReferenceType.Parameter:
                        return this.Components.Parameters[reference.Id];

                    case ReferenceType.Example:
                        return this.Components.Examples[reference.Id];

                    case ReferenceType.RequestBody:
                        return this.Components.RequestBodies[reference.Id];

                    case ReferenceType.Header:
                        return this.Components.Headers[reference.Id];

                    case ReferenceType.SecurityScheme:
                        return this.Components.SecuritySchemes[reference.Id];

                    case ReferenceType.Link:
                        return this.Components.Links[reference.Id];

                    case ReferenceType.Callback:
                        return this.Components.Callbacks[reference.Id];

                    default:
                        throw new OpenApiException(Properties.SRResource.InvalidReferenceType);
                }
            } catch(KeyNotFoundException)
            {
                throw new OpenApiException(string.Format(Properties.SRResource.InvalidReferenceId,reference.Id));
            }
        }
    }
}