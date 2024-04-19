namespace Tectonic;

public abstract class ActivityBase<TInput, TOutput> : IActivity<TInput, TOutput>
{
    /// <inheritdoc />
    public Type InputType { get; } = typeof(TInput);

    /// <inheritdoc />
    public Type OutputType { get; } = typeof(TOutput);

    /// <inheritdoc />
    public async Task<object?> ExecuteNonGeneric(object input)
    {
        if(input is not TInput tInput)
            throw new ArgumentException($"Expected a {typeof(TInput).FullName} input: {input.GetType()}", nameof(input));
        return await Execute(tInput);
    }

    /// <summary>
    /// Execute the activity.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>
    /// The output.
    /// </returns>
    public abstract Task<TOutput?> Execute(TInput input);
}
