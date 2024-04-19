using System.Reflection;
using Microsoft.Extensions.Logging;
using StudioLE.Patterns;

namespace Tectonic.StaticMethodActivities;

/// <summary>
/// Build an <see cref="StaticMethodActivityProvider"/>.
/// </summary>
public class StaticMethodActivityProviderBuilder : IBuilder<StaticMethodActivityProvider>
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly Dictionary<string, MethodInfo> _activityTypes = new();

    /// <summary>
    /// DI constructor for <see cref="StaticMethodActivityProviderBuilder"/>.
    /// </summary>
    public StaticMethodActivityProviderBuilder(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Add all <see cref="IActivity"/> from <paramref name="assembly"/>.
    /// </summary>
    /// <paramref name="assembly">The assembly.</paramref>
    /// <returns>
    /// The <see cref="StaticMethodActivityProvider"/> for fluent chaining.
    /// </returns>
    public StaticMethodActivityProviderBuilder Add(Assembly assembly)
    {
        IEnumerable<MethodInfo> methods = GetActivityMethods(assembly);
        foreach (MethodInfo method in methods)
        {
            string key = GetActivityKey(method);
            _activityTypes[key] = method;
        }
        return this;
    }

    /// <inheritdoc/>
    public StaticMethodActivityProvider Build()
    {
        return new(_loggerFactory, _activityTypes);
    }

    /// <summary>
    /// Get all activity methods in the assembly.
    /// </summary>
    private static IEnumerable<MethodInfo> GetActivityMethods(Assembly assembly)
    {
        IEnumerable<Type> types = assembly
            .GetTypes()
            .Where(TypeFilter);
        IEnumerable<MethodInfo> methods = types
            .SelectMany(type => type
                .GetMethods()
                .Where(method => method.DeclaringType == type)
                .Where(MethodFilter));
        return methods;
    }

    /// <summary>
    /// Get the activity key.
    /// </summary>
    private static string GetActivityKey(MemberInfo member)
    {
        string? @namespace = member
            .DeclaringType
            ?.FullName;
        return @namespace is null
            ? member.Name
            : $"{@namespace}.{member.Name}";
    }

    /// <summary>
    /// Get an activity method from the assembly by key.
    /// </summary>
    internal static MethodInfo? GetActivityMethodByKey(Assembly assembly, string activityKey)
    {
        IEnumerable<MethodInfo> methods = GetActivityMethods(assembly);
        return methods.FirstOrDefault(method => GetActivityKey(method) == activityKey);
    }

    private static bool TypeFilter(Type type)
    {
        return type is
        {
            IsPublic: true,
            IsClass: true
        };
    }

    private static bool MethodFilter(MethodBase method)
    {
        return method is
        {
            IsPublic: true,
            IsStatic: true,
            IsAbstract: false,
            IsVirtual: false
        };
    }
}
