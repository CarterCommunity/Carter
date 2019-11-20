namespace Carter.OpenApi
{
    using System;
    using System.Net.Http;

    /// <summary>
    /// A class to supply OpenApi data
    /// </summary>
    public abstract class RouteMetaData
    {
        /// <summary>
        /// A description about the route
        /// </summary>
        public virtual string Description { get; }

        /// <summary>
        /// A tag to group routes by
        /// </summary>
        public virtual string Tag { get; }

        /// <summary>
        /// An array of <see cref="RouteMetaDataRequest"/>s that must be sent.
        /// </summary>
        public virtual RouteMetaDataRequest[] Requests { get; }

        /// <summary>
        /// An array of possible <see cref="RouteMetaDataResponse"/>s that can be returned from the route
        /// </summary>
        public virtual RouteMetaDataResponse[] Responses { get; }
        
        /// <summary>
        /// A unique identifier for the route
        /// </summary>
        public virtual string OperationId { get; }

        /// <summary>
        /// The name of a security schema used in the API
        /// </summary>
        public virtual string SecuritySchema { get; set; }
        
        /// <summary>
        /// An array of possible <see cref="QueryStringParameter"/>s that a route may use
        /// </summary>
        public virtual QueryStringParameter[] QueryStringParameter { get; }

        /// <summary>
        /// The <see cref="HttpContent"/> type associated with a request.
        /// </summary>
        public virtual Type Content { get; }
    }
}