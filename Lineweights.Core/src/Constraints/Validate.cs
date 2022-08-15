using Ardalis.Result;

namespace Lineweights.Core.Constraints;

/// <summary>
/// Methods to validate constraints.
/// </summary>
public static class Validate
{
    /// <summary>
    /// Validate the <paramref name="constraint"/>.
    /// Throw an exception if it fails.
    /// </summary>
    public static T OrThrow<T>(Constraint<T> constraint)
    {
        if (constraint.IsValid())
            return constraint.Value;
        throw constraint.Exception();
    }

    /// <summary>
    /// Validate the <paramref name="result"/>.
    /// Throw an exception if it is not a success.
    /// </summary>
    public static T OrThrow<T>(Result<T> result, string contextMessage)
    {
        if (result.IsSuccess)
            return result;
        string message = contextMessage;
        if (result.Errors.Any())
            message += Environment.NewLine + string.Join(Environment.NewLine, result.Errors);
        throw new(message);
    }

    /// <summary>
    /// Validate <paramref name="obj"/> is of type <typeparamref name="T"/>.
    /// Throw an exception if it fails.
    /// </summary>
    public static T IsTypeOrThrow<T>(object obj, string contextMessage)
    {
        if (obj is T tValue)
            return tValue;
        string message = contextMessage;
        message += $" Expected the {obj.GetType()} to be a {nameof(T)}.";
        throw new(message);
    }
}
