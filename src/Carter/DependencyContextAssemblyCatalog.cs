namespace Carter;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyModel;

public class DependencyContextAssemblyCatalog
{
    private static readonly string fluentValidationAssemblyName;

    private static readonly string carterAssemblyName;

    private readonly DependencyContext dependencyContext;

    private readonly Assembly[] overriddenAssemblies = Array.Empty<Assembly>();

    static DependencyContextAssemblyCatalog()
    {
        fluentValidationAssemblyName = typeof(IValidator).Assembly.GetName().Name;
        carterAssemblyName = typeof(CarterExtensions).Assembly.GetName().Name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyContextAssemblyCatalog"/> class,
    /// using <see cref="Assembly.GetEntryAssembly()"/>.
    /// </summary>
    public DependencyContextAssemblyCatalog()
        : this(Assembly.GetEntryAssembly())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyContextAssemblyCatalog"/> class,
    /// using <paramref name="entryAssembly"/>.
    /// </summary>
    public DependencyContextAssemblyCatalog(Assembly entryAssembly)
    {
        this.dependencyContext = DependencyContext.Load(entryAssembly);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyContextAssemblyCatalog"/> class,
    /// using assemblies passed in to disable dependency scanning.
    /// </summary>
    public DependencyContextAssemblyCatalog(params Assembly[] assemblies)
    {
        this.overriddenAssemblies = assemblies;
    }

    /// <summary>
    /// Gets all <see cref="Assembly"/> instances in the catalog.
    /// </summary>
    /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Assembly"/> instances.</returns>
    public virtual IReadOnlyCollection<Assembly> GetAssemblies()
    {
        var results = new HashSet<Assembly>
        {
            typeof(DependencyContextAssemblyCatalog).Assembly
        };

        if (this.overriddenAssemblies.Any())
        {
            foreach (var overriddenAssembly in this.overriddenAssemblies)
            {
                results.Add(overriddenAssembly);
            }
        }
        else
        {
            foreach (var library in this.dependencyContext.RuntimeLibraries)
            {
                if (IsReferencingCarter(library) || IsReferencingFluentValidation(library))
                {
                    foreach (var assemblyName in library.GetDefaultAssemblyNames(this.dependencyContext))
                    {
                        results.Add(SafeLoadAssembly(assemblyName));
                    }
                }
            }
        }

        return results;
    }

    private static Assembly SafeLoadAssembly(AssemblyName assemblyName)
    {
        try
        {
            return Assembly.Load(assemblyName);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static bool IsReferencingCarter(Library library)
    {
        return library.Dependencies.Any(dependency => dependency.Name.Equals(carterAssemblyName));
    }

    private static bool IsReferencingFluentValidation(Library library)
    {
        return library.Dependencies.Any(dependency => dependency.Name.Equals(fluentValidationAssemblyName));
    }
}
