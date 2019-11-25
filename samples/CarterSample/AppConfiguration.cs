namespace CarterSample
{
    public class AppConfiguration
    {
        public static string ConnectionString { get; set; }

        public CarterOptions CarterOptions { get; set; }
    }

    public class CarterOptions
    {
        public OpenApi OpenApi { get; set; }
    }

    public class OpenApi
    {
        public string DocumentTitle { get; set; }
    }
}
