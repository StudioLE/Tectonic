namespace Lineweights.Core.Constraints;

/// <summary>
/// A constraint to validate if a condition passes.
/// </summary>
public sealed class UnhandledConditionConstraint : Constraint<Func<bool>>
{
    private readonly string _message;

    /// <inheritdoc/>
    public UnhandledConditionConstraint(Func<bool> condition, string message) : base(condition, null)
    {
        _message = message;
    }

    /// <inheritdoc/>
    public override bool IsValid()
    {
        return Value();
    }

    /// <inheritdoc/>
    public override string Message()
    {
        return _message;
    }

    /// <inheritdoc/>
    public override Exception Exception()
    {
        return new(Message());
    }
}
