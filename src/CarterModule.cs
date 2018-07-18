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
        public readonly Dictionary<(string verb, string path), CarterRoute> Routes;

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
            this.Routes = new Dictionary<(string verb, string path), CarterRoute>(RouteComparer.Comparer);
            this.basePath = RemoveEndingSlash(RemoveStartingSlash(basePath));
        }

        /// <summary>
        /// Declares a route for GET requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Get(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) 
            => this.Get(path, ctx => handler(ctx.Request, ctx.Response, ctx.GetRouteData()));

        /// <summary>
        /// Declares a route for GET requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Get(string path, RequestDelegate handler) 
            => this.AddRoute(path, handler, HttpMethods.Get, HttpMethods.Head);

        /// <summary>
        /// Declares a route for POST requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Post(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) 
            => this.Post(path, ctx => handler(ctx.Request, ctx.Response, ctx.GetRouteData()));

        /// <summary>
        /// Declares a route for POST requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Post(string path, RequestDelegate handler) 
            => this.AddRoute(path, handler, HttpMethods.Post);

        /// <summary>
        /// Declares a route for DELETE requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Delete(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) 
            => this.Delete(path, ctx => handler(ctx.Request, ctx.Response, ctx.GetRouteData()));

        /// <summary>
        /// Declares a route for DELETE requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Delete(string path, RequestDelegate handler) 
            => this.AddRoute(path, handler, HttpMethods.Delete);

        /// <summary>
        /// Declares a route for PUT requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Put(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) 
            => this.Put(path, ctx => handler(ctx.Request, ctx.Response, ctx.GetRouteData()));

        /// <summary>
        /// Declares a route for PUT requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Put(string path, RequestDelegate handler) => this.AddRoute(path, handler, HttpMethods.Put);

        /// <summary>
        /// Declares a route for HEAD requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Head(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) 
            => this.Head(path, ctx => handler(ctx.Request, ctx.Response, ctx.GetRouteData()));

        /// <summary>
        /// Declares a route for HEAD requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Head(string path, RequestDelegate handler) 
            => this.AddRoute(path, handler, HttpMethods.Head);

        /// <summary>
        /// Declares a route for PATCH requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Patch(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) 
            => this.Patch(path, ctx => handler(ctx.Request, ctx.Response, ctx.GetRouteData()));

        /// <summary>
        /// Declares a route for PATCH requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Patch(string path, RequestDelegate handler) 
            => this.AddRoute(path, handler, HttpMethods.Patch);

        /// <summary>
        /// Declares a route for OPTIONS requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Options(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) 
            => this.Options(path, ctx => handler(ctx.Request, ctx.Response, ctx.GetRouteData()));

        /// <summary>
        /// Declares a route for OPTIONS requests
        /// </summary>
        /// <param name="path">The path for your route</param>
        /// <param name="handler">The handler that is invoked when the route is hit</param>
        protected void Options(string path, RequestDelegate handler) 
            => this.AddRoute(path, handler, HttpMethods.Options);

        private void AddRoute(string path, RequestDelegate handler, params string[] verbs)
        {
            path = this.PrependBasePath(RemoveStartingSlash(path));
            foreach (var verb in verbs)
            {
                async Task CompositeHandler(HttpContext ctx)
                {
                    var shouldContinue = true;

                    if (this.Before != null)
                    {
                        shouldContinue = await this.Before(ctx);
                    }

                    if (shouldContinue)
                    {
                        await handler(ctx);

                        if (this.After != null)
                        {
                            await this.After(ctx);
                        }
                    }
                }

                this.Routes.Add((verb, path), new CarterRoute(verb, path, this.GetType(), CompositeHandler));
            }
        }

        private static string RemoveStartingSlash(string path) 
            => path.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? path.Substring(1) : path;

        private static string RemoveEndingSlash(string path) 
            => path.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? path.Remove(path.Length - 1) : path;

        private string PrependBasePath(string path) 
            => string.IsNullOrEmpty(this.basePath) ? path : $"{this.basePath}/{path}";

        /// <summary>
        /// Case-insensitive comparer for routes.
        /// </summary>
        private class RouteComparer : IEqualityComparer<(string verb, string path)>
        {
            /// <summary>
            /// Shared comparer instance.
            /// </summary>
            public static readonly RouteComparer Comparer = new RouteComparer();

            public bool Equals((string verb, string path) x, (string verb, string path) y)
                => StringComparer.OrdinalIgnoreCase.Equals(x.verb, y.verb) && StringComparer.OrdinalIgnoreCase.Equals(x.path, y.path);

            public int GetHashCode((string verb, string path) obj)
                => (StringComparer.OrdinalIgnoreCase.GetHashCode(obj.verb), StringComparer.OrdinalIgnoreCase.GetHashCode(obj.path)).GetHashCode();
        }
    }
}
