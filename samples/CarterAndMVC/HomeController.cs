namespace CarterAndMVC
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        // GET
        public string Index()
        {
            return "Hello from MVC";
        }
    }
    
    
}
