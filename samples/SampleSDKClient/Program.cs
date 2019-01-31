namespace SampleSDKClient
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            //dotnet dotnet-nswag.dll swagger2csclient /input:http://localhost:5000/openapi /namespace:SampleSDKClient /usebaseurl:false /responsearraytype:System.Collections.Generic.IEnumerable /RequiredPropertiesMustBeDefined:true /output:/Users/jonathan/Projects/Carter/samples/SampleSDKClient/SDK.cs

            var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

            var actorsClient = new ActorsClient(httpClient);

            var actors = await actorsClient.GetActorsAsync();

            foreach (var actor in actors)
            {
                Console.WriteLine(actor.Id + Environment.NewLine + actor.Name + Environment.NewLine + actor.Age);
            }
        }
    }
}
