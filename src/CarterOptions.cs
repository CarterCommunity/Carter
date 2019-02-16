namespace Carter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// A class that allows you to provide options to configure Carter
    /// </summary>
    public class CarterOptions
    {
        /// <summary>
        /// Initializes <see cref="CarterOptions"/>
        /// </summary>
        /// <param name="before">A global before handler which is invoked before all routes</param>
        /// <param name="after">A global after handler which is invoked after all routes</param>
        public CarterOptions(Func<HttpContext, Task<bool>> before = null, Func<HttpContext, Task> after = null, OpenApiOptions openApiOptions = null)
        {
            this.Before = before;
            this.After = after;
            this.OpenApi = openApiOptions ?? new OpenApiOptions("Carter <3 OpenApi", Enumerable.Empty<string>(), new Dictionary<string, OpenApiSecurity>());
        }

        /// <summary>
        /// A global before handler which is invoked before all routes
        /// </summary>
        public Func<HttpContext, Task<bool>> Before { get; }

        /// <summary>
        /// A global after handler which is invoked after all routes
        /// </summary>
        public Func<HttpContext, Task> After { get; }

        /// <summary>
        /// Options for configuring the OpenAPI response
        /// </summary>
        public OpenApiOptions OpenApi { get; }
    }

    public class OpenApiOptions
    {
        public OpenApiOptions(string documentTitle, IEnumerable<string> addresses, Dictionary<string, OpenApiSecurity> securityDefinitions, IEnumerable<string> globalSecurityDefinitions = null)
        {
            this.DocumentTitle = documentTitle;
            this.ServerUrls = addresses;
            this.Securities = securityDefinitions;
            this.GlobalSecurityDefinitions = globalSecurityDefinitions ?? Enumerable.Empty<string>();
        }

        public string DocumentTitle { get; }

        public IEnumerable<string> ServerUrls { get; }

        public Dictionary<string, OpenApiSecurity> Securities { get; }
        
        public IEnumerable<string> GlobalSecurityDefinitions { get; }
    }

    public class OpenApiSecurity
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Scheme { get; set; }

        public string BearerFormat { get; set; }

        public string In { get; set; }
    }
}
