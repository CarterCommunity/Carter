namespace CarterSample
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    class Program
    {
        static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}