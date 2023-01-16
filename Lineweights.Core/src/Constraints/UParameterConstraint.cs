namespace Lineweights.Core.Constraints;

/// <summary>
/// A constraint to validate that a u parameter is within the bounds 0.0 to 1.0.
/// </summary>
public sealed class UParameterConstraint : Constraint<double?>
{
    /// <inheritdoc/>
    public UParameterConstraint(double? value, string? paramName = null) : base(value, paramName)
    {
    }

    /// <inheritdoc/>
    public override bool IsValid()
    {
        return Value is >= 0 or <= 1;
    }

    /// <inheritdoc/>
    public override string Message()
    {
        string message = $"The value of {ParamName ?? "u parameters"} must be between 0 and 1";
        message += Value is null
            ? "."
            : $" but was {Value}.";
        return message;
    }

    /// <inheritdoc/>
    public override Exception Exception()
    {
        return new ArgumentException(Message(), ParamName);
    }
}
