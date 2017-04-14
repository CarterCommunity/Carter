namespace Botwin.Samples
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;

    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new WebHostBuilder()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseKestrel()
                    .UseStartup<Startup>()
                    .UseUrls("http://*:5000")
                    .Build())
            {
                Console.WriteLine("Starting application...");
                host.Run();
            }
        }
    }
}