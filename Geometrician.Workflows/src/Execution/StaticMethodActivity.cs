using System.Reflection;

namespace Geometrician.Workflows.Execution;

/// <summary>
/// An <see cref="IActivity"/> based on a static <see cref="MethodInfo"/> in an <see cref="Assembly"/>.
/// </summary>
public sealed class StaticMethodActivity : IActivity
{
    private readonly object? _instance;
    private readonly MethodInfo _method;

    /// <inheritdoc/>
    public string Key { get; }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public string Description { get; set; }

    /// <inheritdoc/>
    public object[] Inputs { get; private set; } = Array.Empty<object>();

    /// <summary>
    /// Construct a <see cref="StaticMethodActivity"/> from <see cref="method"/>.
    /// </summary>
    /// <param name="instance">The instance the method belongs to. For static methods this should be null.</param>
    /// <param name="method">The method to be executed by the activity.</param>
    /// <param name="key">The unique key identifying the activity.</param>
    public StaticMethodActivity(object? instance, MethodInfo method, string key)
    {
        _instance = instance;
        _method = method;
        Key = key;
        Name = Key;
        Description = Key;
        Reset();
    }

    /// <inheritdoc/>
    public Task<object> Execute()
    {
        object? output = _method.Invoke(_instance, Inputs);
        return output is Task<object> task
            ? task
            : Task.FromResult(output);
    }

    private void Reset()
    {
        Inputs = _method
            .GetParameters()
            .Select(x => Activator.CreateInstance(x.ParameterType))
            .ToArray();
    }

    /// <inheritdoc/>
    public object Clone()
    {
        return new StaticMethodActivity(_instance, _method, Key)
        {
            Name = Name,
            Description = Description
        };
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}
