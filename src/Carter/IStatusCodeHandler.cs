namespace Carter
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Provides informative responses for particular HTTP status codes
    /// </summary>
    public interface IStatusCodeHandler
    {
        /// <summary>
        /// Check if the error handler can handle errors of the provided status code.
        /// </summary>
        /// <param name="statusCode">The HTTP status code</param>
        /// <returns>A <see cref="bool"/> indicating if it can be handled</returns>
        bool CanHandle(int statusCode);
        
        /// <summary>
        /// The handler for the status code
        /// </summary>
        /// <param name="ctx">The current <see cref="HttpContext"/></param>
        /// <returns>A <see cref="Task"/></returns>
        Task Handle(HttpContext ctx);
    }
}