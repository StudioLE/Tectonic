namespace StudioLE.Core.Results;

public class Failure : IResult
{
    /// <inheritdoc />
    public string[] Warnings { get; set; } = Array.Empty<string>();

    public string[] Errors { get; }

    public Exception? Exception { get; }

    [Obsolete("Failure should contain at least one error.")]
    public Failure()
    {
        Errors = new[] { "An unspecified error occurred." };
    }

    public Failure(params string[] errors)
    {
        Errors = errors.Any()
            ? errors
            : new[] { "An unspecified error occurred." };
    }

    public Failure(string error, params string[] errors)
    {
        Errors = errors.Prepend(error).ToArray();
    }

    public Failure(IResult<object> result, params string[] error)
    {
        Errors = error
            .Concat(result.Errors)
            .ToArray();
        Warnings = result.Warnings;
    }

    public Failure(Exception exception, params string[] errors)
    {
        Exception = exception;
        Errors = errors.Prepend(exception.Message).ToArray();
    }

    public Failure(string error, Exception exception)
    {
        Exception = exception;
        Errors = new[] { error, exception.Message };
    }
}

public class Failure<T> : IResult<T>
{
    /// <inheritdoc />
    public string[] Warnings { get; set; } = Array.Empty<string>();

    public string[] Errors { get; }

    public Exception? Exception { get; }

    [Obsolete("Failure should contain at least one error.")]
    public Failure()
    {
        Errors = new[] { "An unspecified error occurred." };
    }

    public Failure(params string[] errors)
    {
        if (!errors.Any())
            throw new("Failure must contain at least one error.");
        Errors = errors;
    }

    public Failure(string error, params string[] errors)
    {
        Errors = errors.Prepend(error).ToArray();
    }

    public Failure(IResult<object> result, params string[] error)
    {
        Errors = error
            .Concat(result.Errors)
            .ToArray();
        Warnings = result.Warnings;
    }

    public Failure(Exception exception, params string[] errors)
    {
        Exception = exception;
        Errors = errors.Prepend(exception.Message).ToArray();
    }

    public Failure(string error, Exception exception)
    {
        Exception = exception;
        Errors = new[] { error, exception.Message };
    }
}
