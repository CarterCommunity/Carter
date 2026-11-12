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

    private readonly Assembly entryAssembly;

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
        this.entryAssembly = entryAssembly;
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

            return results;
        }

        var foundInDependencyContext = false;

        if (this.dependencyContext is not null)
        {
            foreach (var library in this.dependencyContext.RuntimeLibraries)
            {
                if (IsReferencingCarter(library) || IsReferencingFluentValidation(library))
                {
                    foundInDependencyContext = true;

                    foreach (var assemblyName in library.GetDefaultAssemblyNames(this.dependencyContext))
                    {
                        results.Add(SafeLoadAssembly(assemblyName));
                    }
                }
            }
        }

        // In single-file published apps the DependencyContext is either null (self-contained)
        // or only describes the shared framework (framework-dependent), so nothing is found above.
        // In that case, walk the assembly references of the entry assembly instead.
        if (!foundInDependencyContext && this.entryAssembly is not null)
        {
            foreach (var assembly in GetReferencedAssemblies(this.entryAssembly))
            {
                results.Add(assembly);
            }
        }

        return results;
    }

    /// <summary>
    /// Walks the assembly references of <paramref name="rootAssembly"/> and returns the assemblies,
    /// including <paramref name="rootAssembly"/>, that reference Carter or FluentValidation.
    /// </summary>
    /// <remarks>Framework assemblies are neither loaded nor walked.</remarks>
    internal static IReadOnlyCollection<Assembly> GetReferencedAssemblies(Assembly rootAssembly)
    {
        var results = new List<Assembly>();
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { rootAssembly.GetName().Name };
        var queue = new Queue<Assembly>();
        queue.Enqueue(rootAssembly);

        while (queue.Count > 0)
        {
            var assembly = queue.Dequeue();
            var references = assembly.GetReferencedAssemblies();

            if (references.Any(reference => reference.Name == carterAssemblyName || reference.Name == fluentValidationAssemblyName))
            {
                results.Add(assembly);
            }

            foreach (var reference in references)
            {
                if (IsFrameworkAssembly(reference.Name) || !visited.Add(reference.Name))
                {
                    continue;
                }

                var referencedAssembly = SafeLoadAssembly(reference);
                if (referencedAssembly is not null)
                {
                    queue.Enqueue(referencedAssembly);
                }
            }
        }

        return results;
    }

    private static bool IsFrameworkAssembly(string assemblyName)
    {
        return assemblyName.StartsWith("System.", StringComparison.Ordinal)
               || assemblyName.StartsWith("Microsoft.", StringComparison.Ordinal)
               || assemblyName is "System" or "mscorlib" or "netstandard";
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
