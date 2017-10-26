namespace Botwin
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class BotwinModule
    {
        public readonly List<(string verb, string path, RequestDelegate handler)> Routes;

        private readonly string basePath;

        public Func<HttpContext, Task<bool>> Before { get; set; } 

        public RequestDelegate After { get; protected set; }

        protected BotwinModule() : this(string.Empty)
        {
        }

        protected BotwinModule(string basePath)
        {
            this.Routes = new List<(string verb, string path, RequestDelegate handler)>();
            var cleanPath = this.RemoveStartingSlash(basePath);
            this.basePath = this.RemoveEndingSlash(cleanPath);
        }

        protected void Get(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Get(path, RequestDelegate);
        }

        protected void Get(string path, RequestDelegate handler)
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Get, path, handler));
            this.Routes.Add((HttpMethods.Head, path, handler));
        }

        protected void Post(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Post(path, RequestDelegate);
        }

        protected void Post(string path, RequestDelegate handler)
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Post, path, handler));
        }

        protected void Delete(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Delete(path, RequestDelegate);
        }

        protected void Delete(string path, RequestDelegate handler)
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Delete, path, handler));
        }

        protected void Put(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Put(path, RequestDelegate);
        }

        protected void Put(string path, RequestDelegate handler)
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Put, path, handler));
        }

        protected void Head(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Head(path, RequestDelegate);
        }

        protected void Head(string path, RequestDelegate handler)
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Head, path, handler));
        }

        protected void Patch(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());

            this.Patch(path, RequestDelegate);
        }

        protected void Patch(string path, RequestDelegate handler)
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Patch, path, handler));
        }

        protected void Options(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            Task RequestDelegate(HttpContext httpContext) => handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Options(path, RequestDelegate);
        }

        protected void Options(string path, RequestDelegate handler)
        {
            path = this.RemoveStartingSlash(path);
            path = this.PrependBasePath(path);
            this.Routes.Add((HttpMethods.Options, path, handler));
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
    }
}
