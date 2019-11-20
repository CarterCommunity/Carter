namespace Carter.Request
{
    using Microsoft.AspNetCore.Routing;

    public static class RouteValueDictionaryExtensions
    {
        public static T As<T>(this RouteData routeData, string key)
        {
            var value = routeData.Values[key];

            return value.ConvertTo<T>();
        }
        
        /// <summary>
        /// Retrieve strongly typed route value for given key
        /// </summary>
        /// <param name="routeValueDictionary"><see cref="RouteValueDictionary"/></param>
        /// <param name="key">Route key</param>
        /// <typeparam name="T">Route value data type</typeparam>
        /// <returns>Route value for given key</returns>
        public static T As<T>(this RouteValueDictionary routeValueDictionary, string key)
        {
            var value = routeValueDictionary[key];

            return value.ConvertTo<T>();
        }
    }
}