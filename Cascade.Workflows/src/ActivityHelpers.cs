using System.ComponentModel;
using System.Reflection;

namespace Cascade.Workflows;

public static class ActivityHelpers
{
    private const string ExecuteMethodName = "Execute";

    private static MethodInfo GetExecuteMethodOrThrow(IActivity activity)
    {
        Type type = activity.GetType();
        MethodInfo? method = type.GetMethod(ExecuteMethodName);
        if (method is null)
            throw new($"Expected {nameof(IActivity)} to have an {ExecuteMethodName} method.");
        if (method.GetParameters().Length != 1)
            throw new($"Expected {nameof(IActivity)} {ExecuteMethodName} method to accept a single parameter.");
        return method;
    }

    public static Task<object> Execute(this IActivity activity, object input)
    {
        MethodInfo method = GetExecuteMethodOrThrow(activity);
        object[] parameters =  { input };
        object? result = method.Invoke(activity, parameters);
        return result is Task task
            ? task.Cast<object>()
            : Task.FromResult(result);
    }

    public static string GetName(this IActivity activity)
    {
        if (activity is IActivityMetadata metadata)
            return metadata.Name;
        Type type = activity.GetType();
        DisplayNameAttribute? attribute = type.GetCustomAttribute<DisplayNameAttribute>();
        return attribute.DisplayName;
    }

    public static string GetDescription(this IActivity activity)
    {
        if (activity is IActivityMetadata metadata)
            return metadata.Description;
        Type type = activity.GetType();
        DescriptionAttribute? attribute = type.GetCustomAttribute<DescriptionAttribute>();
        return attribute.Description;
    }

    public static Type GetInputType(this IActivity activity)
    {
        MethodInfo method = GetExecuteMethodOrThrow(activity);
        return method.GetParameters().First().ParameterType;
    }

    public static Type GetOutputType(this IActivity activity)
    {
        MethodInfo method = GetExecuteMethodOrThrow(activity);
        return method.ReturnType;
    }

    /// <summary>
    /// Casts a <see cref="Task"/> to a <see cref="Task{TResult}"/>.
    /// This method will throw an <see cref="InvalidCastException"/> if the specified task
    /// returns a value which is not identity-convertible to <typeparamref name="T"/>.
    /// </summary>
    /// <see href="https://stackoverflow.com/a/53471924/247218">Source</see>
    private static async Task<T> Cast<T>(this Task task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));
        if (!task.GetType().IsGenericType || task.GetType().GetGenericTypeDefinition() != typeof(Task<>))
            throw new ArgumentException("An argument of type 'System.Threading.Tasks.Task`1' was expected");

        await task.ConfigureAwait(false);

        object result = task
            .GetType()
            .GetProperty(nameof(Task<object>.Result))
            !.GetValue(task);
        return (T)result;
    }

}
