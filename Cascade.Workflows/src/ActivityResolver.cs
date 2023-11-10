using System.Reflection;
using StudioLE.Core.Results;

namespace Cascade.Workflows;

/// <inheritdoc cref="IActivityResolver"/>
public sealed class ActivityResolver : IActivityResolver
{
    // TODO: Simplify this by removing all the IResult

    /// <inheritdoc/>
    public IEnumerable<string> AllActivityKeysInAssembly(Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(TypeFilter)
            .Select(GetActivityKey);
    }

    private static Type? ResolveType(Assembly assembly, string activityKey)
    {
        return assembly
            .GetTypes()
            .Where(TypeFilter)
            .FirstOrDefault(x => GetActivityKey(x) == activityKey);
    }

    /// <inheritdoc/>
    public IResult<IActivity> Resolve(Assembly assembly, string activityKey)
    {
        Type? type = ResolveType(assembly, activityKey);
        if (type is null)
            return new Failure<IActivity>("Failed to find by key.");
        IActivity activity = ActivityFactory.Create(type);
        return new Success<IActivity>(activity);
    }

    private static bool TypeFilter(Type type)
    {
        return type.IsPublic
               && type.IsClass
               && type.GetInterface(nameof(IActivity)) is not null;
    }

    private static string GetActivityKey(Type type)
    {
        string assemblyPrefix = type.Assembly.GetName().Name;
        string @namespace = type.FullName ?? string.Empty;
        if (@namespace.StartsWith(assemblyPrefix))
            @namespace = @namespace.Remove(0, assemblyPrefix.Length + 1);
        return $"{@namespace}.{type.Name}";
    }
}
