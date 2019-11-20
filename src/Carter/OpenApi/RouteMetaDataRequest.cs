namespace Carter.OpenApi
{
    using System;

    /// <summary>
    /// A class to describe an OpenApi request object
    /// </summary>
    public class RouteMetaDataRequest
    {
        /// <summary>
        /// The media type.
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// A description of the request
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The <see cref="Type"/> of request body
        /// </summary>
        public Type Request { get; set; }

        /// <summary>
        /// The format of the request
        /// </summary>
        public string Format { get; set; }
    }
}