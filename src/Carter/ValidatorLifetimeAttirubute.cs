namespace Carter;

using System;
using Microsoft.Extensions.DependencyInjection;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ValidatorLifetimeAttribute(ServiceLifetime lifetime) : Attribute
{
    public ServiceLifetime Lifetime { get; } = lifetime;
}
