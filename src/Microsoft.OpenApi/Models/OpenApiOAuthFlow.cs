// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// OAuth Flow Object.
    /// </summary>
    public class OpenApiOAuthFlow : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. The authorization URL to be used for this flow.
        /// Applies to implicit and authorizationCode OAuthFlow.
        /// </summary>
        public Uri AuthorizationUrl { get; set; }

        /// <summary>
        /// REQUIRED. The token URL to be used for this flow.
        /// Applies to password, clientCredentials, and authorizationCode OAuthFlow.
        /// </summary>
        public Uri TokenUrl { get; set; }

        /// <summary>
        /// The URL to be used for obtaining refresh tokens.
        /// </summary>
        public Uri RefreshUrl { get; set; }

        /// <summary>
        /// REQUIRED. A map between the scope name and a short description for it.
        /// </summary>
        public IDictionary<string, string> Scopes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // authorizationUrl
            writer.WriteProperty(OpenApiConstants.AuthorizationUrl, AuthorizationUrl?.ToString());

            // tokenUrl
            writer.WriteProperty(OpenApiConstants.TokenUrl, TokenUrl?.ToString());

            // refreshUrl
            writer.WriteProperty(OpenApiConstants.RefreshUrl, RefreshUrl?.ToString());

            // scopes
            writer.WriteRequiredMap(OpenApiConstants.Scopes, Scopes, (w, s) => w.WriteValue(s));

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v3.0
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // authorizationUrl
            await writer.WritePropertyAsync(OpenApiConstants.AuthorizationUrl, AuthorizationUrl?.ToString());

            // tokenUrl
            await writer.WritePropertyAsync(OpenApiConstants.TokenUrl, TokenUrl?.ToString());

            // refreshUrl
            await writer.WritePropertyAsync(OpenApiConstants.RefreshUrl, RefreshUrl?.ToString());

            // scopes
            await writer.WriteRequiredMapAsync(OpenApiConstants.Scopes, Scopes, async (w, s) => await w.WriteValueAsync(s));

            // extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }


        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // OAuthFlow object does not exist in V2.
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v2.0
        /// </summary>
        public Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            return Task.CompletedTask;
        }
    }
}