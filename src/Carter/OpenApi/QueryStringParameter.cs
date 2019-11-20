namespace Carter.OpenApi
{
    using System;

    /// <summary>
    /// A class to describe the OpenApi querystring parameters
    /// </summary>
    public class QueryStringParameter
    {
        /// <summary>
        /// The name of the querystring parameter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is the querystring parameter required?
        /// </summary>
        public bool  Required { get; set; }

        /// <summary>
        /// A description of the querystring parameter
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// The <see cref="Type"/> of the querystring parameter
        /// </summary>
        public Type Type { get; set; }
    }
}