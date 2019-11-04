namespace Carter.OpenApi
{
    using System;

    /// <summary>
    /// A class to describe an OpenApi response object
    /// </summary>
    public class RouteMetaDataResponse
    {
        /// <summary>
        /// The status code of the response
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// A description about the response
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The <see cref="Type"/> of response body
        /// </summary>
        public Type Response { get; set; }
    }
}