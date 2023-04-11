namespace Geometrician.Workflows.Execution;

public interface IActivity : ICloneable, IDisposable
{
    /// <summary>
    /// The unique key to identify the activity.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// The name of the activity.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The description of the activity.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// The inputs to use when executing the activity.
    /// </summary>
    public object[] Inputs { get; }

    /// <summary>
    /// The execution method of the activity.
    /// </summary>
    /// <returns>The outputs of the activity execution.</returns>
    public Task<object> Execute();
}
