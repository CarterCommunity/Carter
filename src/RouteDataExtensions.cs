namespace Botwin
{
    using System;
    using Microsoft.AspNetCore.Routing;

    public static class RouteDataExtensions
    {
        public static int AsInt(this RouteData routeData, string key)
        {
            return Convert.ToInt32(routeData.Values[key]);
        }
    }
}