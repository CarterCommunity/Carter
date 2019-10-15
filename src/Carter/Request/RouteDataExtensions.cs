namespace Carter.Request
{
    using Microsoft.AspNetCore.Routing;

    public static class RouteDataExtensions
    {
        /// <summary>
        /// Retrieve strongly typed route value for given key
        /// </summary>
        /// <typeparam name="T">Route value data type</typeparam>
        /// <param name="routeData"><see cref="RouteData"/></param>
        /// <param name="key">Route key</param>
        /// <returns>Route value for given key</returns>
        public static T As<T>(this RouteData routeData, string key)
        {
            var value = routeData.Values[key];

            return value.ConvertTo<T>();
        }
    }
}