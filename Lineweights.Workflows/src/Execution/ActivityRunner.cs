using System.Reflection;
using Ardalis.Result;

namespace Lineweights.Workflows.Execution;

public class ActivityRunner : IActivityRunner
{
    private IReadOnlyCollection<MethodInfo> _activities = Array.Empty<MethodInfo>();

    /// <inheritdoc/>
    public Result<IReadOnlyCollection<string>> ExtractActivities(Assembly assembly)
    {
        Type[] types = assembly
            .GetTypes()
            .Where(type => type.IsPublic
                           && type.IsClass)
            .ToArray();

        var methods = types
            .SelectMany(type => type
                .GetMethods()
                .Where(method => method.DeclaringType == type
                                 && method.IsPublic
                                 && method.IsStatic
                                 && !method.IsAbstract
                                 && !method.IsVirtual))
            .ToArray();

        _activities = methods
            .ToArray();

        if (_activities.Count == 0)
            return Result<IReadOnlyCollection<string>>.Error("No activities were found.");

        // TODO: Return key value pair
        return _activities
            .Select(ActivityId)
            .ToArray();
    }

    /// <inheritdoc/>
    public Result<object?> ExtractInputs(string activityId)
    {
        MethodInfo? activity = GetActivity(activityId);
        if (activity is null)
            return Result<object?>.Error("Activity not found.");
        ParameterInfo[] parameters = activity.GetParameters();

        return parameters.Length switch
        {
            1 => Activator.CreateInstance(parameters.First().ParameterType),
            0 => Result<object?>.Success(null),
            _ => Result<object?>.Error("Activity had multiple parameters.")
        };
    }

    /// <inheritdoc/>
    public Result<object> Execute(string activityId)
    {
        MethodInfo? activity = GetActivity(activityId);
        if (activity is null)
            return Result<object>.Error("Activity not found.");

        var inputs = ExtractInputs(activityId);
        if (!inputs.IsSuccess)
            return Result<object>.Error(inputs.Errors.ToArray());
        object[] parameters = inputs.Value is null
            ? Array.Empty<object>()
            : new [] { inputs.Value };
        try
        {
            object? outputs = activity.Invoke(null, parameters);
            return outputs;
        }
        catch (Exception e)
        {
            return Result<object>.Error($"Failed to execute activity. A {e.GetType()} exception was thrown: {e.Message}");
        }
    }

    private MethodInfo? GetActivity(string activityId)
    {
        return _activities.FirstOrDefault(method => ActivityId(method) == activityId);
    }

    private static string ActivityId(MethodInfo method)
    {
        return $"{method.DeclaringType?.FullName}.{method.Name}";
    }
}
