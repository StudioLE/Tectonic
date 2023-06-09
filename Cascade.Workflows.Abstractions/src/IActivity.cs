namespace Cascade.Workflows;

public interface IActivity
{
}

public interface IActivityMetadata
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
}

public interface IActivity<in TInput, TOutput> : IActivity
{
    /// <summary>
    /// The execution method of the activity.
    /// </summary>
    /// <returns>The outputs of the activity execution.</returns>
    public Task<TOutput> Execute(TInput input);
}
