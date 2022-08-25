namespace Lineweights.Workflows.Execution;

/// <summary>
/// An executable <see href="https://refactoring.guru/design-patterns/command">command</see>.
/// The activity is defined as the
/// </summary>
public class ActivityCommand
{
    /// <summary>
    /// The name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The inputs.
    /// </summary>
    public object[] Inputs { get; }

    /// <summary>
    /// The activity called by the execution method.
    /// </summary>
    private readonly Func<object[], object> _activity;

    /// <inheritdoc cref="ActivityCommand"/>
    public ActivityCommand(string name, object[] inputs, Func<object[], object> activity)
    {
        Name = name;
        Inputs = inputs;
        _activity = activity;
    }

    /// <summary>
    /// The execute the command.
    /// </summary>
    public object Execute()
    {
        return _activity.Invoke(Inputs);
    }
}
