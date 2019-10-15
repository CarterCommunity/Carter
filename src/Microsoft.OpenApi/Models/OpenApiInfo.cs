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
    /// Open API Info Object, it provides the metadata about the Open API.
    /// </summary>
    public class OpenApiInfo : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. The title of the application.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A short description of the application.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// REQUIRED. The version of the OpenAPI document.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// A URL to the Terms of Service for the API. MUST be in the format of a URL.
        /// </summary>
        public Uri TermsOfService { get; set; }

        /// <summary>
        /// The contact information for the exposed API.
        /// </summary>
        public OpenApiContact Contact { get; set; }

        /// <summary>
        /// The license information for the exposed API.
        /// </summary>
        public OpenApiLicense License { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize <see cref="OpenApiInfo"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // title
            writer.WriteProperty(OpenApiConstants.Title, Title);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // termsOfService
            writer.WriteProperty(OpenApiConstants.TermsOfService, TermsOfService?.OriginalString);

            // contact object
            writer.WriteOptionalObject(OpenApiConstants.Contact, Contact, (w, c) => c.SerializeAsV3(w));

            // license object
            writer.WriteOptionalObject(OpenApiConstants.License, License, (w, l) => l.SerializeAsV3(w));

            // version
            writer.WriteProperty(OpenApiConstants.Version, Version);

            // specification extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiInfo"/> to Open Api v3.0
        /// </summary>
        public async Task SerializeAsV3Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // title
            await writer.WritePropertyAsync(OpenApiConstants.Title, Title);

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);

            // termsOfService
            await writer.WritePropertyAsync(OpenApiConstants.TermsOfService, TermsOfService?.OriginalString);

            // contact object
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Contact, Contact, async (w, c) => await c.SerializeAsV3Async(w));

            // license object
            await writer.WriteOptionalObjectAsync(OpenApiConstants.License, License, async (w, l) => await l.SerializeAsV3Async(w));

            // version
            await writer.WritePropertyAsync(OpenApiConstants.Version, Version);

            // specification extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi3_0);

            await writer.WriteEndObjectAsync();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiInfo"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // title
            writer.WriteProperty(OpenApiConstants.Title, Title);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // termsOfService
            writer.WriteProperty(OpenApiConstants.TermsOfService, TermsOfService?.OriginalString);

            // contact object
            writer.WriteOptionalObject(OpenApiConstants.Contact, Contact, (w, c) => c.SerializeAsV2(w));

            // license object
            writer.WriteOptionalObject(OpenApiConstants.License, License, (w, l) => l.SerializeAsV2(w));

            // version
            writer.WriteProperty(OpenApiConstants.Version, Version);

            // specification extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiInfo"/> to Open Api v2.0
        /// </summary>
        public async Task SerializeAsV2Async(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            await writer.WriteStartObjectAsync();

            // title
            await writer.WritePropertyAsync(OpenApiConstants.Title, Title);

            // description
            await writer.WritePropertyAsync(OpenApiConstants.Description, Description);

            // termsOfService
            await writer.WritePropertyAsync(OpenApiConstants.TermsOfService, TermsOfService?.OriginalString);

            // contact object
            await writer.WriteOptionalObjectAsync(OpenApiConstants.Contact, Contact, async (w, c) => await c.SerializeAsV2Async(w));

            // license object
            await writer.WriteOptionalObjectAsync(OpenApiConstants.License, License, async (w, l) => await l.SerializeAsV2Async(w));

            // version
            await writer.WritePropertyAsync(OpenApiConstants.Version, Version);

            // specification extensions
            await writer.WriteExtensionsAsync(Extensions, OpenApiSpecVersion.OpenApi2_0);

            await writer.WriteEndObjectAsync();
        }
    }
}