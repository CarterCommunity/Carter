namespace Carter
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// A class for defining routes in your Carter application
    /// </summary>
    public class CarterModule
    {
        public readonly Dictionary<(string verb, string path), RequestDelegate> Routes;

        private readonly string basePath;

        /// <summary>
        /// A handler that can be invoked before the defined route
        /// </summary>
        public Func<HttpContext, Task<bool>> Before { get; set; }

        /// <summary>
        /// A handler that can be invoked after the defined route
        /// </summary>
        public RequestDelegate After { get; protected set; }

        /// <summary>
        /// Initializes a new instance of <see cref="CarterModule"/>
        /// </summary>
        protected CarterModule() : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CarterModule"/>
        /// </summary>
        /// <param name="basePath">A base path to group routes in your <see cref="CarterModule"/></param>
        protected CarterModule(string basePath)
        {
            this.Routes = new Dictionary<(string verb, string path), RequestDelegate>(RouteComparer.Comparer);
            var cleanPath = this.RemoveStartingSlash(basePath);
            this.basePath = this.RemoveEndingSlash(cleanPath);
        }

        /// <summary>
        /// Declares a route for GET requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Get(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Get(path, RequestDelegate);
        }

        /// <summary>
        /// Declares a route for GET requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Get(string path, RequestDelegate handler)
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Get, path), handler);
            this.Routes.Add((HttpMethods.Head, path), handler);
        }

        /// <summary>
        /// Declares a route for POST requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Post(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Post(path, RequestDelegate);
        }

        /// <summary>
        /// Declares a route for POST requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>

        protected void Post(string path, RequestDelegate handler)
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Post, path), handler);
        }

        /// <summary>
        /// Declares a route for DELETE requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Delete(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Delete(path, RequestDelegate);
        }

        /// <summary>
        /// Declares a route for DELETE requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Delete(string path, RequestDelegate handler)
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Delete, path), handler);
        }

        /// <summary>
        /// Declares a route for PUT requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Put(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Put(path, RequestDelegate);
        }

        /// <summary>
        /// Declares a route for PUT requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Put(string path, RequestDelegate handler)
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Put, path), handler);
        }

        /// <summary>
        /// Declares a route for HEAD requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Head(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Head(path, RequestDelegate);
        }

        /// <summary>
        /// Declares a route for HEAD requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Head(string path, RequestDelegate handler)
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Head, path), handler);
        }

        /// <summary>
        /// Declares a route for PATCH requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Patch(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());

            this.Patch(path, RequestDelegate);
        }

        /// <summary>
        /// Declares a route for PATCH requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Patch(string path, RequestDelegate handler)
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Patch, path), handler);
        }

        /// <summary>
        /// Declares a route for OPTIONS requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Options(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Options(path, RequestDelegate);
        }

        /// <summary>
        /// Declares a route for OPTIONS requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Options(string path, RequestDelegate handler)
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Options, path), handler);
        }

        private string RemoveStartingSlash(string path)
        {
            return path.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? path.Substring(1) : path;
        }

        private string RemoveEndingSlash(string path)
        {
            return path.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? path.Remove(path.Length - 1) : path;
        }

        private string PrependBasePath(string path)
        {
            if (string.IsNullOrEmpty(this.basePath))
            {
                return path;
            }

            return $"{this.basePath}/{path}";
        }

        /// <summary>
        /// Case-insensitive comparer for routes.
        /// </summary>
        private class RouteComparer : IEqualityComparer<(string verb, string path)>
        {
            /// <summary>
            /// Shared comparer instance.
            /// </summary>
            public static RouteComparer Comparer = new RouteComparer();

            public bool Equals((string verb, string path) x, (string verb, string path) y)
                => StringComparer.OrdinalIgnoreCase.Equals(x.verb, y.verb) && StringComparer.OrdinalIgnoreCase.Equals(x.path, y.path);

            public int GetHashCode((string verb, string path) obj)
                => (StringComparer.OrdinalIgnoreCase.GetHashCode(obj.verb), StringComparer.OrdinalIgnoreCase.GetHashCode(obj.path)).GetHashCode();
        }
    }
}
