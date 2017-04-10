# Botwin

Botwin is a library that allows [Nancy-esque](http://nancyfx.org) routing for use with ASP.Net Core. 

This is not a framework, it simply builds on top of [Microsoft.AspNetCore.Routing](https://github.com/aspnet/Routing) allowing you to have more elegant routing rather than have attribute routing, convention routing, ASP.Net Controllers or IRouteBuilder extensions. 

For a better undestanding take a look at the [samples](https://github.com/jchannon/Botwin/tree/master/samples) inside this repo.  The samples also provide elegant extensions around common ASP.Net Core types as shown below.


```
public class ActorsModule : BotwinModule
{
    public ActorsModule(IPersonProvider personProvider)
    {
        this.Get("/actors", async (req, res, routeData) =>
        {
            var people = personProvider.Get();
            await res.WriteAsync(JsonConvert.SerializeObject(people));
        });

        this.Get("/actors/{id:int}", async (req, res, routeData) =>
        {
            var person = personProvider.Get(routeData.Values.AsInt("id"));
            await res.Negotiate(person);
        });
    }
}
```