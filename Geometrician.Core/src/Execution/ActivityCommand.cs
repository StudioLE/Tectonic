namespace Geometrician.Core.Execution;

/// <summary>
/// An executable <see href="https://refactoring.guru/design-patterns/command">command</see>.
/// The activity is defined as the
/// </summary>
public class ActivityCommand : IDisposable
{
    /// <summary>
    /// The name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The unique key to identify the command.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// The inputs.
    /// </summary>
    public object[] Inputs { get; set; } = Array.Empty<object>();

    /// <summary>
    /// The activity called by the execution method.
    /// </summary>
    public Func<object[], object>? Invocation { get; set; }

    public Action? OnDispose { get; set; }

    /// <summary>
    /// The execute the command.
    /// </summary>
    public object Execute()
    {
        if (Inputs is null)
            throw new InvalidOperationException($"Failed to execute the activity command. {nameof(Inputs)} has not been set.");
        if (Invocation is null)
            throw new InvalidOperationException($"Failed to execute the activity command. {nameof(Inputs)} has not been set.");
        return Invocation.Invoke(Inputs);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        OnDispose?.Invoke();
    }
}
