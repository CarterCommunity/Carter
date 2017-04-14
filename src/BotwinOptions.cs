namespace Botwin
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class BotwinOptions
    {
        public BotwinOptions(Func<HttpContext, Task<bool>> before = null, Func<HttpContext, Task> after = null)
        {
            this.Before = before;
            this.After = after;
        }
        public Func<HttpContext, Task<bool>> Before { get; }

        public Func<HttpContext, Task> After { get; }
    }
}