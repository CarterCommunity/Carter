// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Security Requirement Object.
    /// Each name MUST correspond to a security scheme which is declared in
    /// the Security Schemes under the Components Object.
    /// If the security scheme is of type "oauth2" or "openIdConnect",
    /// then the value is a list of scope names required for the execution.
    /// For other security scheme types, the array MUST be empty.
    /// </summary>
    public class OpenApiSecurityRequirement : Dictionary<OpenApiSecurityScheme, IList<string>>,
        IOpenApiSerializable
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiSecurityRequirement"/> class.
        /// This constructor ensures that only Reference.Id is considered when two dictionary keys
        /// of type <see cref="OpenApiSecurityScheme"/> are compared.
        /// </summary>
        public OpenApiSecurityRequirement()
            : base(new OpenApiSecuritySchemeReferenceEqualityComparer())
        {
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            foreach (var securitySchemeAndScopesValuePair in this)
            {
                var securityScheme = securitySchemeAndScopesValuePair.Key;
                var scopes = securitySchemeAndScopesValuePair.Value;

                if (securityScheme.Reference == null)
                {
                    // Reaching this point means the reference to a specific OpenApiSecurityScheme fails.
                    // We are not able to serialize this SecurityScheme/Scopes key value pair since we do not know what
                    // string to output.
                    continue;
                }

                securityScheme.SerializeAsV3(writer);

                writer.WriteStartArray();

                foreach (var scope in scopes)
                {
                    writer.WriteValue(scope);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> to Open Api v3.0
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            foreach (var securitySchemeAndScopesValuePair in this)
            {
                var securityScheme = securitySchemeAndScopesValuePair.Key;
                var scopes = securitySchemeAndScopesValuePair.Value;

                if (securityScheme.Reference == null)
                {
                    // Reaching this point means the reference to a specific OpenApiSecurityScheme fails.
                    // We are not able to serialize this SecurityScheme/Scopes key value pair since we do not know what
                    // string to output.
                    continue;
                }

                await securityScheme.SerializeAsV3Async(writer);

                await writer.WriteStartArrayAsync();

                foreach (var scope in scopes)
                {
                    await writer.WriteValueAsync(scope);
                }

                await writer.WriteEndArrayAsync();
            }

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            foreach (var securitySchemeAndScopesValuePair in this)
            {
                var securityScheme = securitySchemeAndScopesValuePair.Key;
                var scopes = securitySchemeAndScopesValuePair.Value;

                if (securityScheme.Reference == null)
                {
                    // Reaching this point means the reference to a specific OpenApiSecurityScheme fails.
                    // We are not able to serialize this SecurityScheme/Scopes key value pair since we do not know what
                    // string to output.
                    continue;
                }

                securityScheme.SerializeAsV2(writer);

                writer.WriteStartArray();

                foreach (var scope in scopes)
                {
                    writer.WriteValue(scope);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> to Open Api v2.0
        /// </summary>
        public async Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            foreach (var securitySchemeAndScopesValuePair in this)
            {
                var securityScheme = securitySchemeAndScopesValuePair.Key;
                var scopes = securitySchemeAndScopesValuePair.Value;

                if (securityScheme.Reference == null)
                {
                    // Reaching this point means the reference to a specific OpenApiSecurityScheme fails.
                    // We are not able to serialize this SecurityScheme/Scopes key value pair since we do not know what
                    // string to output.
                    continue;
                }

                await securityScheme.SerializeAsV2Async(writer);

                await writer.WriteStartArrayAsync();

                foreach (var scope in scopes)
                {
                    await writer.WriteValueAsync(scope);
                }

                await writer.WriteEndArrayAsync();
            }

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Comparer for OpenApiSecurityScheme that only considers the Id in the Reference
        /// (i.e. the string that will actually be displayed in the written document)
        /// </summary>
        private class OpenApiSecuritySchemeReferenceEqualityComparer : IEqualityComparer<OpenApiSecurityScheme>
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            public bool Equals(OpenApiSecurityScheme x, OpenApiSecurityScheme y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                if (x.Reference == null || y.Reference == null)
                {
                    return false;
                }

                return x.Reference.Id == y.Reference.Id;
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            public int GetHashCode(OpenApiSecurityScheme obj)
            {
                return obj?.Reference?.Id == null ? 0 : obj.Reference.Id.GetHashCode();
            }
        }
    }
}