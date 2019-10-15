// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Attributes;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The type of the security scheme
    /// </summary>
    public enum SecuritySchemeType
    {
        /// <summary>
        /// Use API key
        /// </summary>
        [Display("apiKey")] ApiKey,

        /// <summary>
        /// Use basic or bearer token authorization header.
        /// </summary>
        [Display("http")] Http,

        /// <summary>
        /// Use OAuth2
        /// </summary>
        [Display("oauth2")] OAuth2,

        /// <summary>
        /// Use OAuth2 with OpenId Connect URL to discover OAuth2 configuration value.
        /// </summary>
        [Display("openIdConnect")] OpenIdConnect
    }
}