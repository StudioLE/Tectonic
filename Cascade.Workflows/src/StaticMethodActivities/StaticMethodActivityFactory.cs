using System.Reflection;

namespace Cascade.Workflows.StaticMethodActivities;

public static class StaticMethodActivityFactory
{
    /// <summary>
    /// Construct a <see cref="StaticMethodActivity{TInput,TOutput}"/> from a <see cref="method"/>.
    /// </summary>
    /// <param name="method">The method to be executed by the activity.</param>
    /// <param name="key">The unique key identifying the activity.</param>
    public static IActivity Create(MethodInfo method, string key)
    {
        if (method.GetParameters().Length != 1)
            throw new($"Expected {nameof(IActivity)} method to accept only a single parameter.");
        Type inputType = method.GetParameters().First().ParameterType;
        Type outputType = method.ReturnType;
        Type createdType = typeof(StaticMethodActivity<,>).MakeGenericType(inputType, outputType);
        object? createdInstance = Activator.CreateInstance(createdType, method, key);
        if (createdInstance is not IActivity activity)
            throw new($"Failed to construct activity. Activator didn't return an {nameof(IActivity)}.");
        return activity;
    }
}
