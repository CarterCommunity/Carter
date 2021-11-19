namespace Carter;

using Microsoft.AspNetCore.Routing;

/// <summary>
/// An interface to define HTTP routes
/// </summary>
public interface ICarterModule
{
    /// <summary>
    /// Invoked at startup to add routes to the HTTP pipeline
    /// </summary>
    /// <param name="app">An instance of <see cref="IEndpointRouteBuilder"/></param>
    void AddRoutes(IEndpointRouteBuilder app);
}