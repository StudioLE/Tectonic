using System.Reflection;
using StudioLE.Core.Results;

namespace Lineweights.Workflows.Execution;

/// <inheritdoc cref="IActivityFactory"/>
public sealed class StaticMethodActivityFactory : IActivityFactory
{
    /// <inheritdoc />
    public IEnumerable<string> AllActivityKeysInAssembly(Assembly assembly)
    {
        IEnumerable<MethodInfo> methods = AllActivityMethodsInAssembly(assembly);
        return methods.Select(GetActivityKey);
    }

    /// <inheritdoc />
    public IResult<ActivityCommand> TryCreateByKey(Assembly assembly, string activityKey)
    {
        MethodInfo? method = GetActivityMethodByKey(assembly, activityKey);
        if (method is null)
            return new Failure<ActivityCommand>("No activity in the assembly matched the key.");
        object[] inputs = CreateParameterInstances(method);
        Func<object[], object> invocation = CreateInvocation(method);
        return new Success<ActivityCommand>(new()
        {
            Name = activityKey,
            Key = activityKey,
            Inputs = inputs,
            Invocation = invocation,

        });
    }

    /// <summary>
    /// Get the activity key.
    /// For <see cref="MemberInfo"/> activities the key is the full name of the method
    /// without the prefix of its assembly.
    /// </summary>
    private static string GetActivityKey(MemberInfo member)
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
        return method
            .GetParameters()
            .Select(x => Activator.CreateInstance(x.ParameterType))
            .ToArray();
    }

    /// <summary>
    /// Get all activity methods in the assembly.
    /// </summary>
    internal static IEnumerable<MethodInfo> AllActivityMethodsInAssembly(Assembly assembly)
    {
        IEnumerable<Type> types = assembly
            .GetTypes()
            .Where(TypeFilter);

        IEnumerable<MethodInfo> methods = types
            .SelectMany(type => type
                .GetMethods()
                .Where(method => MethodFilter(method, type)));

        return methods;
    }

    /// <summary>
    /// Get an activity method from the assembly by key.
    /// </summary>
    internal static MethodInfo? GetActivityMethodByKey(Assembly assembly, string activityKey)
    {
        IEnumerable<MethodInfo> methods = AllActivityMethodsInAssembly(assembly);
        return methods.FirstOrDefault(method => GetActivityKey(method) == activityKey);
    }

    private static Func<object[], object> CreateInvocation(MethodBase method)
    {
        return inputs => method.Invoke(null, inputs);
    }

    private static bool TypeFilter(Type type)
    {
        return type.IsPublic
               && type.IsClass;
    }

    private static bool MethodFilter(MethodBase method, Type type)
    {
        return method.DeclaringType == type
               && method.IsPublic
               && method.IsStatic
               && !method.IsAbstract
               && !method.IsVirtual;
    }
}
