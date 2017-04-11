namespace Botwin
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class BotwinModule
    {
        public List<Tuple<string, string, Func<HttpRequest, HttpResponse, RouteData, Task>>> Routes { get; } = new List<Tuple<string, string, Func<HttpRequest, HttpResponse, RouteData, Task>>>();

        public void Get(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            path = path.StartsWith("/") ? path.Substring(1) : path;
            this.Routes.Add(Tuple.Create("GET", path, handler));
        }

        public void Post(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            path = path.StartsWith("/") ? path.Substring(1) : path;
            this.Routes.Add(Tuple.Create("POST", path, handler));
        }

        public void Delete(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            path = path.StartsWith("/") ? path.Substring(1) : path;
            this.Routes.Add(Tuple.Create("DELETE", path, handler));
        }

        public void Put(string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler)
        {
            path = path.StartsWith("/") ? path.Substring(1) : path;
            this.Routes.Add(Tuple.Create("PUT", path, handler));
        }

        public Func<HttpRequest, HttpResponse, RouteData, Task> Before { get; set; }

        public Func<HttpRequest, HttpResponse, RouteData, Task> After { get; set; }
    }
}