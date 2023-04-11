namespace Geometrician.Core.Constraints;

/// <summary>
/// A constraint used by <see cref="Validate"/>.
/// </summary>
public abstract class Constraint<TValue>
{
    /// <summary>
    /// The value to be test..
    /// </summary>
    public TValue Value { get; set; } = default!;

    /// <summary>
    /// The param name of the value.
    /// </summary>
    public string? ParamName { get; set; }

    /// <inheritdoc cref="Constraint{TValue}"/>
    protected Constraint(TValue value, string? paramName)
    {
        Value = value;
        ParamName = paramName;
    }

    /// <summary>
    /// Determine if <see cref="Value"/> is valid.
    /// </summary>
    public abstract bool IsValid();

    /// <summary>
    /// The message used on failure.
    /// </summary>
    public abstract string Message();

    /// <summary>
    /// The exception to throw on failure.
    /// </summary>
    public abstract Exception Exception();
}
