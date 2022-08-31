using System.Reflection;

namespace Lineweights.Workflows.Execution;

/// <summary>
/// Methods to help with activities.
/// </summary>
public static class ActivityHelpers
{
    /// <summary>
    /// Get an activity by name.
    /// </summary>
    internal static MethodInfo? GetActivityMethodByKey(Assembly assembly, string activityKey)
    {
        return GetActivityMethods(assembly)
            .FirstOrDefault(method => GetActivityKey(method) == activityKey);
    }

    /// <summary>
    /// Get all activities from the assembly.
    /// </summary>
    internal static IEnumerable<MethodInfo> GetActivityMethods(Assembly assembly)
    {
        IEnumerable<Type> types = assembly
            .GetTypes()
            .Where(type => type.IsPublic
                           && type.IsClass);

        IEnumerable<MethodInfo> methods = types
            .SelectMany(type => type
                .GetMethods()
                .Where(method => method.DeclaringType == type
                                 && method.IsPublic
                                 && method.IsStatic
                                 && !method.IsAbstract
                                 && !method.IsVirtual));

        return methods;
    }

    /// <summary>
    /// Get the activity key.
    /// For <see cref="MemberInfo"/> activities the key is the full name of the method
    /// without the prefix of its assembly.
    /// </summary>
    internal static string GetActivityKey(MemberInfo member)
    {
        Type type = member.DeclaringType ?? throw new("Failed to get activity key. The method didn't have a declaring type.");
        string assemblyPrefix = type.Assembly.GetName().Name;
        string @namespace = type.FullName ?? string.Empty;
        if (@namespace.StartsWith(assemblyPrefix))
            @namespace = @namespace.Remove(0, assemblyPrefix.Length + 1);
        return $"{@namespace}.{member.Name}";
    }

    /// <summary>
    /// Get instances of the method parameters.
    /// </summary>
    internal static object[] CreateParameterInstances(MethodInfo method)
    {
        return  method
            .GetParameters()
            .Select(x => Activator.CreateInstance(x.ParameterType))
            .ToArray();
    }
}
