namespace Botwin
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class BotwinModule
    {
        public List<(string verb, string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)> Routes = new List<(string verb, string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)>();
       
        public Func<HttpRequest, HttpResponse, RouteData, Task<HttpResponse>> Before { get; set; }

        public Func<HttpRequest, HttpResponse, RouteData, Task> After { get; set; }

        public void Get(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            path = path.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? path.Substring(1) : path;
            this.Routes.Add((HttpMethods.Get, path, handler));
            this.Routes.Add((HttpMethods.Head, path, handler));
        }

        public void Post(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            path = path.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? path.Substring(1) : path;
            this.Routes.Add((HttpMethods.Post, path, handler));
        }

        public void Delete(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            path = path.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? path.Substring(1) : path;
            this.Routes.Add((HttpMethods.Delete, path, handler));
        }

        public void Put(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            path = path.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? path.Substring(1) : path;
            this.Routes.Add((HttpMethods.Put, path, handler));
        }

        public void Head(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            path = path.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? path.Substring(1) : path;
            this.Routes.Add((HttpMethods.Head, path, handler));
        }
    }
}