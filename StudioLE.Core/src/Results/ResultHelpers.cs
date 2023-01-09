namespace StudioLE.Core.Results;

public static class ResultHelpers
{
    public static T? GetValueAsNullable<T>(this IResult<T> result) where T : class
    {
        return result is Success<T> success
            ? success.Value
            : null;
    }
}
