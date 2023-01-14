using System.Reflection;

namespace Geometrician.Core.Configuration;

/// <summary>
/// A registry of <see cref="Assembly"/> from which activities can be loaded.
/// </summary>
/// <remarks>
/// The <see cref="AssemblyResolver"/> should be constructed via a <see cref="AssemblyResolverBuilder"/> and added to
/// to the dependency injection services.
/// </remarks>
public class AssemblyResolver
{
    private readonly IDictionary<string, Assembly> _assemblies;

    internal AssemblyResolver(IDictionary<string, Assembly> assemblies)
    {
        _assemblies = assemblies;
    }

    /// <summary>
    /// Add <paramref name="assembly"/> to the registry if there is no previous entry with the same name.
    /// </summary>
    /// <param name="assembly">The assembly to add.</param>
    /// <returns>True if the assembly was added, false if the key was already set.</returns>
    public bool TryRegister(Assembly assembly) {
        string name = assembly.GetName().Name;
        if (_assemblies.ContainsKey(name))
            return false;
        _assemblies.Add(name, assembly);
        return true;
    }

    /// <summary>
    /// Get the names of each assembly in the registry.
    /// </summary>
    /// <returns>The names of each assembly in the registry.</returns>
    public IEnumerable<string> GetAllNames()
    {
        return _assemblies.Keys;
    }

    /// <summary>
    /// Get the <see cref="Assembly"/> by <paramref name="assemblyName"/>.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly.</param>
    /// <returns>The resolved <see cref="Assembly"/> if it exists, otherwise <see langword="null"/>.</returns>
    public Assembly? ResolveByName(string assemblyName)
    {
        return _assemblies.TryGetValue(assemblyName, out Assembly assembly)
            ? assembly
            : null;
    }
}
