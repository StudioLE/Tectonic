using System.Diagnostics;
using System.Reflection;

namespace StudioLE.Core.System;

/// <summary>
/// Methods to help with <see cref="Assembly"/>.
/// </summary>
public static class AssemblyHelpers
{
    private static bool _areAllAssembliesLoaded = false;

    /// <summary>
    /// Determine if the calling assembly is a DEBUG build.
    /// </summary>
    public static bool IsDebugBuild()
    {
        return Assembly
                   .GetCallingAssembly()
                   .GetCustomAttribute<DebuggableAttribute>()
                   ?.IsJITTrackingEnabled
               ?? false;
    }

    /// <summary>
    /// Ensure all referenced assemblies are loaded.
    /// </summary>
    public static void LoadAllAssemblies(string? whereStartsWith = null)
    {
        if (_areAllAssembliesLoaded)
            return;

        List<Assembly> loadedAssemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .ToList();

        IEnumerable<AssemblyName> referencedAssemblies = loadedAssemblies
            .SelectMany(x => x.GetReferencedAssemblies())
            .Distinct()
            .Where(x => whereStartsWith == null || x.FullName.StartsWith(whereStartsWith));

        IEnumerable<AssemblyName> notLoadedAssemblies = referencedAssemblies
            .Where(referenced => !loadedAssemblies.Any(loaded => loaded.FullName == referenced.FullName));

        foreach (AssemblyName assembly in notLoadedAssemblies)
            AppDomain.CurrentDomain.Load(assembly);

        _areAllAssembliesLoaded = true;
    }
}
