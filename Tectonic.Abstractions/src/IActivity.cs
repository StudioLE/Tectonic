namespace Tectonic;

public interface IActivity
{
    /// <summary>
    /// The input type.
    /// </summary>
    public Type InputType { get; }

    /// <summary>
    /// The output type.
    /// </summary>
    public Type OutputType { get; }

    /// <summary>
    /// Execute the activity.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>
    /// The output as a Task.
    /// </returns>
    public Task<object?> ExecuteNonGeneric(object input);
}

public interface IActivity<in TInput, TOutput> : IActivity
{
    /// <summary>
    /// The execution method of the activity.
    /// </summary>
    /// <returns>The outputs of the activity execution.</returns>
    public Task<TOutput?> Execute(TInput input);
}
