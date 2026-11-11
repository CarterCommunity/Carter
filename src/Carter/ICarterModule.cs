namespace Carter;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// An interface to define HTTP routes
/// </summary>
/// <remarks>Implementations of <see cref="ICarterModule"/> should not inject constructor dependencies. All dependencies should be supplied in the route <see cref="RequestDelegate"/></remarks>
public interface ICarterModule
{
    /// <summary>
    /// Invoked at startup to add routes to the HTTP pipeline
    /// </summary>
    /// <remarks>Implementations of <see cref="ICarterModule"/> should not inject constructor dependencies. All dependencies should be supplied in the route <see cref="RequestDelegate"/></remarks>
    /// <param name="app">An instance of <see cref="IEndpointRouteBuilder"/></param>
    void AddRoutes(IEndpointRouteBuilder app);
}
