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
            this.OpenApi = openApiOptions ?? new OpenApiOptions(Enumerable.Empty<string>());
        }

        /// <summary>
        /// A global before handler which is invoked before all routes
        /// </summary>
        public Func<HttpContext, Task<bool>> Before { get; }

        /// <summary>
        /// A global after handler which is invoked after all routes
        /// </summary>
        public Func<HttpContext, Task> After { get; }

        public OpenApiOptions OpenApi { get; set; }
    }

    public class OpenApiOptions
    {
        public OpenApiOptions(IEnumerable<string> addresses)
        {
            this.ServerUrls = addresses;
        }

        public string DocumentTitle { get; set; } = "Carter <3 OpenApi";

        public IEnumerable<string> ServerUrls { get; set; }
    }
}
