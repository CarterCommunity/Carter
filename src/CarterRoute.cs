namespace Carter
{
    using System;
    using Microsoft.AspNetCore.Http;

    public class CarterRoute
    {
        public CarterRoute(string verb, string path, Type module, RequestDelegate handler)
        {
            this.Verb = verb;
            this.Path = path;
            this.Module = module;
            this.Handler = handler;
        }
        
        public string Verb { get; }

        public string Path { get; }

        public Type Module { get; }

        public RequestDelegate Handler { get; }
    }
}