using System.Threading;

namespace Botwin
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class BotwinModule
    {
        public List<(string verb, string path, RequestDelegate handler)> Routes =
            new List<(string verb, string path, RequestDelegate handler)>();

        public Func<HttpContext, Task<bool>> Before { get; set; }

        public RequestDelegate After { get; set; }

        public void Get(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            RequestDelegate requestDelegate = httpContext =>
                handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Get(path, requestDelegate);
        }

        public void Get(string path, RequestDelegate handler)
        {
            path = this.RemoveSlash(path);
            this.Routes.Add((HttpMethods.Get, path, handler));
            this.Routes.Add((HttpMethods.Head, path, handler));
        }

        public void Post(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            RequestDelegate requestDelegate = httpContext =>
                handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Post(path, requestDelegate);
        }

        public void Post(string path, RequestDelegate handler)
        {
            path = this.RemoveSlash(path);
            this.Routes.Add((HttpMethods.Post, path, handler));
        }

        public void Delete(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            RequestDelegate requestDelegate = httpContext =>
                handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Delete(path, requestDelegate);
        }

        public void Delete(string path, RequestDelegate handler)
        {
            path = RemoveSlash(path);
            this.Routes.Add((HttpMethods.Delete, path, handler));
        }

        public void Put(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            RequestDelegate requestDelegate = httpContext =>
                handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Put(path, requestDelegate);
        }

        public void Put(string path, RequestDelegate handler)
        {
            path = RemoveSlash(path);
            this.Routes.Add((HttpMethods.Put, path, handler));
        }

        public void Head(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            RequestDelegate requestDelegate = httpContext =>
                handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Head(path, requestDelegate);
        }

        public void Head(string path, RequestDelegate handler)
        {
            path = RemoveSlash(path);
            this.Routes.Add((HttpMethods.Head, path, handler));
        }

        public void Patch(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            RequestDelegate requestDelegate = httpContext =>
                handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());

            this.Patch(path, requestDelegate);
        }

        public void Patch(string path, RequestDelegate handler)
        {
            path = RemoveSlash(path);
            this.Routes.Add((HttpMethods.Patch, path, handler));
        }

        public void Options(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            RequestDelegate requestDelegate = httpContext =>
                handler(httpContext.Request, httpContext.Response, httpContext.GetRouteData());
            this.Options(path, requestDelegate);
        }

        public void Options(string path, RequestDelegate handler)
        {
            path = RemoveSlash(path);
            this.Routes.Add((HttpMethods.Options, path, handler));
        }

        private string RemoveSlash(string path)
        {
            return path.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? path.Substring(1) : path;
        }
    }
}
