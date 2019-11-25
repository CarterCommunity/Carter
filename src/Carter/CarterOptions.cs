namespace Carter
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class that allows you to provide options to configure Carter
    /// </summary>
    public class CarterOptions
    {
        /// <summary>
        /// Initializes <see cref="CarterOptions"/>
        /// </summary>
        public CarterOptions()
        {
            this.OpenApi = new OpenApiOptions();
        }

        /// <summary>
        /// Options for configuring the OpenAPI response
        /// </summary>
        public OpenApiOptions OpenApi { get; set; }
    }

    /// <summary>
    /// A class that allows you to configure OpenApi inside of Carter
    /// </summary>
    public class OpenApiOptions
    {
        /// <summary>
        /// The title of the OpenApi document
        /// </summary>
        public string DocumentTitle { get; set; } = "Carter <3 OpenApi";

        /// <summary>
        /// The servers property of the OpenApi document
        /// </summary>
        public IEnumerable<string> ServerUrls { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The available security definitions of the OpenApi document
        /// </summary>
        public Dictionary<string, OpenApiSecurity> Securities { get; set; } = new Dictionary<string, OpenApiSecurity>();

        /// <summary>
        /// The global security definitions that apply to the api that OpenApi describes
        /// </summary>
        public IEnumerable<string> GlobalSecurityDefinitions { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// A class that describes the OpenApi security definitions
    /// </summary>
    public class OpenApiSecurity
    {
        public OpenApiSecurityType Type { get; set; }

        public string Name { get; set; }

        public string Scheme { get; set; }

        public string BearerFormat { get; set; }

        public OpenApiIn In { get; set; }
    }

    /// <summary>
    /// Location of the apiKey
    /// </summary>
    public enum OpenApiIn
    {
        query,

        header,

        cookie
    }

    /// <summary>
    /// The OpneApi security type
    /// </summary>
    public enum OpenApiSecurityType
    {
        apiKey,

        http,

        oauth2,

        openIdonnect
    }
}
