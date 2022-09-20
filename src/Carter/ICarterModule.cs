namespace Carter;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public abstract class CarterModule : ICarterModule
{
    private IEndpointRouteBuilder app;

    internal string[] hosts;

    internal string corsPolicyName;

    internal string openApiDescription;

    internal object[] metaData;

    internal string openApiName;

    internal string openApisummary;

    internal string openApiDisplayName;

    internal string openApiGroupName;

    internal string[] tags;

    internal bool includeInOpenApi;

    internal bool requiresAuthentication { get; private set; }

    internal string basePath { get; private set; } = "/";

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

    public CarterModule RequireAuthorization()
    {
        this.requiresAuthentication = true;
        return this;
    }

    public abstract void AddRoutes(IEndpointRouteBuilder app);

    public CarterModule RequireHost(params string[] hosts)
    {
        this.hosts = hosts;
        return this;
    }

    public CarterModule RequireCors(string policyName)
    {
        this.corsPolicyName = policyName;
        return this;
    }

    public CarterModule WithDescription(string description)
    {
        this.openApiDescription = description;
        return this;
    }
    
    public CarterModule WithName(string name)
    {
        this.openApiName = name;
        return this;
    }
    
    public CarterModule WithDisplayName(string displayName)
    {
        this.openApiDisplayName = displayName;
        return this;
    }
    
    public CarterModule WithGroupName(string groupName)
    {
        this.openApiGroupName = groupName;
        return this;
    }
    
    public CarterModule WithSummary(string summary)
    {
        this.openApisummary = summary;
        return this;
    }
    
    public CarterModule WithMetadata(params object[] items)
    {
        this.metaData = items;
        return this;
    }
    
    public CarterModule WithTags(params string[] tags)
    {
        this.tags = tags;
        return this;
    }

    public CarterModule IncludeInOpenApi()
    {
        this.includeInOpenApi = true;
        return this;
    }

    public Func<EndpointFilterInvocationContext, IResult> Before { get; set; }

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
