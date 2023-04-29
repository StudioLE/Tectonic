using System.Reflection;
using StudioLE.Core.Patterns;

namespace StudioLE.Workflows.Providers;

/// <summary>
/// Build an <see cref="AssemblyResolver"/>.
/// </summary>
public class AssemblyResolverBuilder : IBuilder<AssemblyResolver>
{
    private readonly Dictionary<string, Assembly> _assemblies = new();

    /// <summary>
    /// Add <paramref name="assembly"/> to the registry.
    /// </summary>
    /// <param name="assembly">The assembly to add.</param>
    /// <returns>The <see cref="AssemblyResolverBuilder"/>.</returns>
    public AssemblyResolverBuilder Register(Assembly assembly)
    {
        string name = assembly.GetName().Name;
        _assemblies.Add(name, assembly);
        return this;
    }

    /// <inheritdoc/>
    public AssemblyResolver Build()
    {
        return new(_assemblies);
    }
}
