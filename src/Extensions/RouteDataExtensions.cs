namespace Botwin.Extensions
{
    using System;
    using Microsoft.AspNetCore.Routing;

    public static class RouteDataExtensions
    {
        public static T As<T>(this RouteData routeData, string key)
        {
            var value = routeData.Values[key];

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}