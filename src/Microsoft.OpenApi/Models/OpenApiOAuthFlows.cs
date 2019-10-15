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
    /// OAuth Flows Object.
    /// </summary>
    public class OpenApiOAuthFlows : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// Configuration for the OAuth Implicit flow
        /// </summary>
        public OpenApiOAuthFlow Implicit { get; set; }

        /// <summary>
        /// Configuration for the OAuth Resource Owner Password flow.
        /// </summary>
        public OpenApiOAuthFlow Password { get; set; }

        /// <summary>
        /// Configuration for the OAuth Client Credentials flow.
        /// </summary>
        public OpenApiOAuthFlow ClientCredentials { get; set; }

        /// <summary>
        /// Configuration for the OAuth Authorization Code flow.
        /// </summary>
        public OpenApiOAuthFlow AuthorizationCode { get; set; }

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlows"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // implicit
            writer.WriteOptionalObject(OpenApiConstants.Implicit, Implicit, (w, o) => o.SerializeAsV3(w));

            // password
            writer.WriteOptionalObject(OpenApiConstants.Password, Password, (w, o) => o.SerializeAsV3(w));

            // clientCredentials
            writer.WriteOptionalObject(
                OpenApiConstants.ClientCredentials,
                ClientCredentials,
                (w, o) => o.SerializeAsV3(w));

            // authorizationCode
            writer.WriteOptionalObject(
                OpenApiConstants.AuthorizationCode,
                AuthorizationCode,
                (w, o) => o.SerializeAsV3(w));

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlows"/> to Open Api v3.0
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // implicit
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Implicit, Implicit, async (w, o) => await o.SerializeAsV3Async(w));

            // password
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Password, Password, async (w, o) => await o.SerializeAsV3Async(w));

            // clientCredentials
            await writer.WriteOptionalObjectAsync(
                OpenApiConstants.ClientCredentials,
                ClientCredentials,
                async (w, o) => await o.SerializeAsV3Async(w));

            // authorizationCode
            await writer .WriteOptionalObjectAsync(
                OpenApiConstants.AuthorizationCode,
                AuthorizationCode,
                async (w, o) => await o.SerializeAsV3Async(w));

            // extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlows"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // OAuthFlows object does not exist in V2.
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlows"/> to Open Api v2.0
        /// </summary>
        public Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            return Task.CompletedTask;
        }
    }
}