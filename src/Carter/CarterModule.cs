namespace Carter
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Carter.OpenApi;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// A class for defining routes in your Carter application
    /// </summary>
    public class CarterModule
    {
        internal readonly Dictionary<(string verb, string path), RequestDelegate> Routes;

        internal readonly Dictionary<(string verb, string path), RouteMetaData> RouteMetaData;

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
            this.RouteMetaData = new Dictionary<(string verb, string path), RouteMetaData>(RouteComparer.Comparer);

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
        /// Declares a route for GET requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        /// <typeparam name="T">The <see cref="OpenApi.RouteMetaData"/> implementation for OpenApi use</typeparam>
        protected void Get<T>(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) where T : RouteMetaData
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Get<T>(path, RequestDelegate);
        }

        /// <summary>
        /// Declares a route for GET requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        /// <typeparam name="T">The <see cref="OpenApi.RouteMetaData"/> implementation for OpenApi use</typeparam>
        protected void Get<T>(string path, RequestDelegate handler) where T : RouteMetaData
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Get, path), handler);
            this.Routes.Add((HttpMethods.Head, path), handler);

            this.RouteMetaData.Add((HttpMethods.Get, path), Activator.CreateInstance<T>());
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
        /// Declares a route for POST requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        /// <typeparam name="T">The <see cref="OpenApi.RouteMetaData"/> implementation for OpenApi use</typeparam>
        protected void Post<T>(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) where T : RouteMetaData
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Post<T>(path, RequestDelegate);
        }

        /// <summary>
        /// Declares a route for POST requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        /// <typeparam name="T">The <see cref="OpenApi.RouteMetaData"/> implementation for OpenApi use</typeparam>
        protected void Post<T>(string path, RequestDelegate handler) where T : RouteMetaData
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Post, path), handler);

            this.RouteMetaData.Add((HttpMethods.Post, path), Activator.CreateInstance<T>());
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
        /// Declares a route for DELETE requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        /// <typeparam name="T">The <see cref="OpenApi.RouteMetaData"/> implementation for OpenApi use</typeparam>
        protected void Delete<T>(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) where T : RouteMetaData
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Delete<T>(path, RequestDelegate);
        }

        /// <summary>
        /// Declares a route for DELETE requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        /// <typeparam name="T">The <see cref="OpenApi.RouteMetaData"/> implementation for OpenApi use</typeparam>
        protected void Delete<T>(string path, RequestDelegate handler) where T : RouteMetaData
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Delete, path), handler);

            this.RouteMetaData.Add((HttpMethods.Delete, path), Activator.CreateInstance<T>());
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
        /// Declares a route for POST requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        /// <typeparam name="T">The <see cref="OpenApi.RouteMetaData"/> implementation for OpenApi use</typeparam>
        protected void Put<T>(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) where T : RouteMetaData
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Put<T>(path, RequestDelegate);
        }

        /// <summary>
        /// Declares a route for POST requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        /// <typeparam name="T">The <see cref="OpenApi.RouteMetaData"/> implementation for OpenApi use</typeparam>
        protected void Put<T>(string path, RequestDelegate handler) where T : RouteMetaData
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Put, path), handler);

            this.RouteMetaData.Add((HttpMethods.Put, path), Activator.CreateInstance<T>());
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
        /// Declares a route for POST requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        /// <typeparam name="T">The <see cref="OpenApi.RouteMetaData"/> implementation for OpenApi use</typeparam>
        protected void Head<T>(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) where T : RouteMetaData
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Head<T>(path, RequestDelegate);
        }

        /// <summary>
        /// Declares a route for POST requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        /// <typeparam name="T">The <see cref="OpenApi.RouteMetaData"/> implementation for OpenApi use</typeparam>
        protected void Head<T>(string path, RequestDelegate handler) where T : RouteMetaData
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Head, path), handler);

            this.RouteMetaData.Add((HttpMethods.Head, path), Activator.CreateInstance<T>());
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
        /// Declares a route for POST requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        /// <typeparam name="T">The <see cref="OpenApi.RouteMetaData"/> implementation for OpenApi use</typeparam>
        protected void Patch<T>(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) where T : RouteMetaData
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Patch<T>(path, RequestDelegate);
        }

        /// <summary>
        /// Declares a route for POST requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        /// <typeparam name="T">The <see cref="OpenApi.RouteMetaData"/> implementation for OpenApi use</typeparam>
        protected void Patch<T>(string path, RequestDelegate handler) where T : RouteMetaData
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Patch, path), handler);

            this.RouteMetaData.Add((HttpMethods.Patch, path), Activator.CreateInstance<T>());
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

        /// <summary>
        /// Declares a route for POST requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        /// <typeparam name="T">The <see cref="OpenApi.RouteMetaData"/> implementation for OpenApi use</typeparam>
        protected void Options<T>(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) where T : RouteMetaData
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Options<T>(path, RequestDelegate);
        }

        /// <summary>
        /// Declares a route for POST requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        /// <typeparam name="T">The <see cref="OpenApi.RouteMetaData"/> implementation for OpenApi use</typeparam>
        protected void Options<T>(string path, RequestDelegate handler) where T : RouteMetaData
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Options, path), handler);

            this.RouteMetaData.Add((HttpMethods.Options, path), Activator.CreateInstance<T>());
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
