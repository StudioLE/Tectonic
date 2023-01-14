using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

    /// <summary>
    /// Construct an <see cref="AssemblyResolver"/> from an existing dictionary.
    /// </summary>
    /// <remarks>
    /// This constructor is used by <see cref="AssemblyResolverBuilder"/>.
    /// </remarks>
    /// <param name="assemblies">The assemblies.</param>
    internal AssemblyResolver(IDictionary<string, Assembly> assemblies)
    {
        _assemblies = assemblies;
    }

    /// <summary>
    /// Construct an <see cref="AssemblyResolver"/> from configuration options.
    /// </summary>
    /// <remarks>
    /// This follows an
    /// <see href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-7.0">options pattern.</see>
    /// </remarks>
    /// <param name="options">The options loaded from configuration.</param>
    /// <param name="logger">The logger.</param>
    public AssemblyResolver(IOptions<ActivitiesOptions> options, ILogger<AssemblyResolver> logger)
    {
        _assemblies = new Dictionary<string, Assembly>();
        foreach (string assemblyPath in options.Value.Assemblies)
        {
            string absolutePath = assemblyPath;
            if(!IsAbsolutePath(absolutePath))
            {
                string assemblyDir = AppDomain.CurrentDomain.BaseDirectory;
                absolutePath = Path.Combine(assemblyDir, absolutePath);
                absolutePath = Path.GetFullPath(absolutePath);
            }
            if (!File.Exists(absolutePath))
            {
                string message = "Failed to load assembly. File not found: " + absolutePath;
                logger.LogError(message);
                return;
            }
            Assembly assembly = Assembly.LoadFrom(absolutePath);
            TryRegister(assembly);
        }
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

    private static bool IsAbsolutePath(string path)
    {
        #if NETSTANDARD2_1
        return Path.IsPathFullyQualified(path)
        #else
        return Path.IsPathRooted(path);
        #endif
    }
}
