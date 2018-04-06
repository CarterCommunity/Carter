namespace Botwin.Samples
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;

    class Program
    {
        static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(x=>x.AddDebug().SetMinimumLevel(LogLevel.Trace))
                .Build()
                .Run();
        }
    }
}