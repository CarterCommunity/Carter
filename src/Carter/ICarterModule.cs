namespace Carter;

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// A base class CarterModule to define settings for all the routes in a module
/// </summary>
public abstract class CarterModule : ICarterModule
{
    internal string[] hosts = Array.Empty<string>();

    internal string corsPolicyName;

    internal string openApiDescription;

    internal object[] metaData = Array.Empty<object>();

    internal string openApiName;

    internal string openApisummary;

    internal string openApiDisplayName;

    internal string openApiGroupName;

    internal string[] tags = Array.Empty<string>();

    internal bool includeInOpenApi;

    internal bool requiresAuthorization;

    internal string[] authorizationPolicyNames;

    internal string cacheOutputPolicyName;

    internal readonly string basePath = "/";

    internal bool disableRateLimiting;

    internal string rateLimitingPolicyName;

    /// <summary>
    /// Initializes a new instance of <see cref="CarterModule"/>
    /// </summary>
    protected CarterModule() : this(string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CarterModule"/>
    /// </summary>
    /// <param name="basePath">A base path to group routes in your <see cref="CarterModule"/></param>
    protected CarterModule(string basePath)
    {
        this.basePath = basePath;
    }

    /// <summary>
    /// Add authorization to all routes
    /// </summary>
    /// <param name="policyNames">
    /// A collection of policy names.
    /// If <c>null</c> or empty, the default authorization policy will be used.
    /// </param>
    /// <returns></returns>
    public CarterModule RequireAuthorization(params string[] policyNames)
    {
        this.requiresAuthorization = true;
        this.authorizationPolicyNames = policyNames;
        return this;
    }

    public abstract void AddRoutes(IEndpointRouteBuilder app);

    /// <summary>
    /// Requires that endpoints match one of the specified hosts during routing.
    /// </summary>
    /// <param name="hosts">The hosts used during routing</param>
    /// <returns></returns>
    public CarterModule RequireHost(params string[] hosts)
    {
        this.hosts = hosts;
        return this;
    }

    /// <summary>
    /// Adds a CORS policy with the specified name to the module's routes.
    /// </summary>
    /// <param name="policyName">The CORS policy name</param>
    /// <returns></returns>
    public CarterModule RequireCors(string policyName)
    {
        this.corsPolicyName = policyName;
        return this;
    }

    /// <summary>
    ///  Adds <see cref="IEndpointDescriptionMetadata"/> to <see cref="EndpointBuilder.Metadata"/> 
    /// </summary>
    /// <param name="description">The description value</param>
    /// <returns></returns>
    public CarterModule WithDescription(string description)
    {
        this.openApiDescription = description;
        return this;
    }

    /// <summary>
    /// Adds the <see cref="IEndpointNameMetadata"/> to the Metadata collection for all endpoints produced
    /// </summary>
    /// <param name="name">The name value</param>
    /// <returns></returns>
    public CarterModule WithName(string name)
    {
        this.openApiName = name;
        return this;
    }

    /// <summary>
    /// Sets the <see cref="EndpointBuilder.DisplayName"/> to the provided <paramref name="displayName"/> for all routes in the module
    /// </summary>
    /// <param name="displayName">The display name value</param>
    /// <returns></returns>
    public CarterModule WithDisplayName(string displayName)
    {
        this.openApiDisplayName = displayName;
        return this;
    }

    /// <summary>
    /// Sets the <see cref="EndpointGroupNameAttribute"/> for all routes for all routes in the module
    /// </summary>
    /// <param name="groupName">The group name value</param>
    /// <returns></returns>
    public CarterModule WithGroupName(string groupName)
    {
        this.openApiGroupName = groupName;
        return this;
    }

    /// <summary>
    /// Adds <see cref="IEndpointSummaryMetadata"/> to <see cref="EndpointBuilder.Metadata"/> for routes in the module
    /// </summary>
    /// <param name="summary">The summary value</param>
    /// <returns></returns>
    public CarterModule WithSummary(string summary)
    {
        this.openApisummary = summary;
        return this;
    }

    /// <summary>
    /// Adds the provided metadata <paramref name="items"/> to <see cref="EndpointBuilder.Metadata"/> for all routes in the module
    /// </summary>
    /// <param name="items">The items to add</param>
    /// <returns></returns>
    public CarterModule WithMetadata(params object[] items)
    {
        this.metaData = items;
        return this;
    }

    /// <summary>
    /// Adds the <see cref="ITagsMetadata"/> to <see cref="EndpointBuilder.Metadata"/> for all routes in the module
    /// </summary>
    /// <param name="tags">The tags to add</param>
    /// <returns></returns>
    public CarterModule WithTags(params string[] tags)
    {
        this.tags = tags;
        return this;
    }

    /// <summary>
    /// Include all routes in the module to the OpenAPI output
    /// </summary>
    /// <returns></returns>
    public CarterModule IncludeInOpenApi()
    {
        this.includeInOpenApi = true;
        return this;
    }

    /// <summary>
    ///  Marks an endpoint to be cached using a named policy.
    /// </summary>
    /// <param name="policyName">The policy name value</param>
    /// <returns></returns>
    public CarterModule WithCacheOutput(string policyName)
    {
        this.cacheOutputPolicyName = policyName;
        return this;
    }

    /// <summary>
    /// Disables rate limiting on all the routes in the module
    /// </summary>
    /// <returns></returns>
    public CarterModule DisableRateLimiting()
    {
        this.disableRateLimiting = true;
        return this;
    }

    /// <summary>
    /// Adds the specified rate limiting policy to all the routes in the module
    /// </summary>
    /// <param name="policyName">The policy name value</param>
    /// <returns></returns>
    public CarterModule RequireRateLimiting(string policyName)
    {
        this.rateLimitingPolicyName = policyName;
        return this;
    }

    /// <summary>
    ///  Registers a filter given a delegate onto all routes in the module
    /// </summary>
    /// <remarks>
    ///  If a non null <see cref="IResult"/> is returned from the delegate, this will be returned and the delegate will not be executed
    /// </remarks>
    public Func<EndpointFilterInvocationContext, IResult> Before { get; set; }

    /// <summary>
    /// Registers a filter given a delegate onto all routes in the module
    /// </summary>
    public Action<EndpointFilterInvocationContext> After { get; set; }
}

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
