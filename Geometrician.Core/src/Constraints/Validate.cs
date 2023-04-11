using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Geometrician.Core.Constraints;

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
    public static T OrThrow<T>(IResult<T> result, string? contextMessage = null)
    {
        if (result is Success<T> success)
            return success;
        string message = contextMessage is null
            ? string.Empty
            : contextMessage + Environment.NewLine;
        if (result.Errors.Any())
            message += result.Errors.Join() + Environment.NewLine;
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
