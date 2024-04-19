using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Tectonic.StaticMethodActivities;

/// <summary>
/// An <see cref="IActivity"/> based on a static <see cref="MethodInfo"/> in an <see cref="Assembly"/>.
/// </summary>
public sealed class StaticMethodActivity : IActivity, IActivityMetadata
{
    private readonly ILogger<StaticMethodActivity> _logger;
    private readonly MethodInfo _method;

    /// <inheritdoc />
    public string Key { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public Type InputType { get; }

    /// <inheritdoc />
    public Type OutputType { get; }

    /// <summary>
    /// Construct a <see cref="StaticMethodActivity"/> from <paramref name="method"/>.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="method">The method to be executed by the activity.</param>
    /// <param name="key">The unique key identifying the activity.</param>
    public StaticMethodActivity(ILogger<StaticMethodActivity> logger, MethodInfo method, string key)
    {
        _logger = logger;
        _method = method;
        Key = key;
        Name = Key;
        Description = Key;
        InputType = _method
            .GetParameters()
            .FirstOrDefault()
            ?.ParameterType ?? throw new("Failed to get input type.");
        OutputType = _method.ReturnType;
    }

    /// <inheritdoc />
    public async Task<object?> ExecuteNonGeneric(object input)
    {
        object[] parameters = [input];
        object? output = _method.Invoke(null, parameters);
        if (output is not Task task)
            return output;
        bool isGeneric = _method.ReturnType.IsGenericType;
        bool isGenericTask = _method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        if (!isGeneric || !isGenericTask)
        {
            _logger.LogCritical($"Failed to execute {nameof(StaticMethodActivity)}: {_method.Name}.");
            _logger.LogInformation($"The return type was a Task but not Task<T>: {output.GetType().FullName}.");
            return null;
        }
        await task.ConfigureAwait(false);
        try
        {
            PropertyInfo? resultProperty = task
                .GetType()
                .GetProperty(nameof(Task<object>.Result));
            if (resultProperty is null)
            {
                _logger.LogCritical($"Failed to execute {nameof(StaticMethodActivity)}: {_method.Name}.");
                _logger.LogInformation($"Failed to get result property of task {task.GetType().FullName}");
                return null;
            }
            object? result = resultProperty.GetValue(task);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, $"Failed to execute {nameof(StaticMethodActivity)}: {_method.Name}.");
            _logger.LogInformation($"An exception was thrown while getting the result of {task.GetType().FullName}");
            // TODO: Change to LogDebug
            _logger.LogInformation($"{e.GetType()}: {e.Message}");
            return null;
        }
    }
}
