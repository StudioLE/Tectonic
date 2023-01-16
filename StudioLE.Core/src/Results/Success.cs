namespace StudioLE.Core.Results;

public class Success : IResult
{
    /// <inheritdoc/>
    public string[] Warnings { get; set; } = Array.Empty<string>();

    /// <inheritdoc/>
    public string[] Errors { get; } = Array.Empty<string>();
}

public class Success<T> : IResult<T>
{
    public T Value { get; }

    /// <inheritdoc/>
    public string[] Warnings { get; set; } = Array.Empty<string>();

    /// <inheritdoc/>
    public string[] Errors { get; } = Array.Empty<string>();

    public Success(T value)
    {
        Value = value;
    }

    public static implicit operator T(Success<T> @this)
    {
        return @this.Value;
    }
}
