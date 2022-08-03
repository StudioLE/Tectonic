namespace Lineweights.Flex.Sequences;

/// <summary>
/// Define if the condition is met by any constraint or all constraints.
/// </summary>
internal enum ConstraintMode
{
    /// <summary>
    /// All constraints must be met for the sequence to end.
    /// </summary>
    And,

    /// <summary>
    /// Any constraint being met will end the sequence.
    /// </summary>
    Or
}

/// <summary>
/// A <see cref="SequenceBuilder"/> constraint.
/// </summary>
internal interface ISequenceConstraint
{
    /// <summary>
    /// Execute the constraint.
    /// </summary>
    public bool Execute(SequenceBuilder sequence, Flex1d builder, double length, int count);
}

/// <summary>
/// A constraint to ensure a pattern does not repeat.
/// </summary>
internal sealed class NonRepeating : ISequenceConstraint
{
    /// <inheritdoc/>
    public bool Execute(SequenceBuilder sequence, Flex1d builder, double length, int count)
    {
        return count <= sequence._items.Count;
    }
}

/// <summary>
/// A constraint to ensure the combined size of a pattern does not exceed a maximum.
/// </summary>
internal sealed class MaxLength : ISequenceConstraint
{
    /// <inheritdoc/>
    public bool Execute(SequenceBuilder sequence, Flex1d builder, double length, int count)
    {
        return length <= builder.TargetLength;
    }
}

/// <summary>
/// A constraint to ensure the number of items in a pattern does not exceed a maximum.
/// </summary>
internal sealed class MaxCount : ISequenceConstraint
{
    private readonly int _maxCount;

    /// <see cref="MaxCount"/>
    public MaxCount(int maxCount)
    {
        _maxCount = maxCount;
    }

    /// <inheritdoc/>
    public bool Execute(SequenceBuilder sequence, Flex1d builder, double length, int count)
    {
        return count + sequence._takeCountAdjustment + sequence._prependedItems.Count + sequence._appendedItems.Count <= _maxCount;
    }
}

/// <summary>
/// A constraint to ensure the total count of a pattern is even.
/// </summary>
internal sealed class IsEven : ISequenceConstraint
{
    /// <inheritdoc/>
    public bool Execute(SequenceBuilder sequence, Flex1d builder, double length, int count)
    {
        return (count + sequence._takeCountAdjustment + sequence._prependedItems.Count + sequence._appendedItems.Count) % 2 == 0;
    }
}

/// <summary>
/// A constraint to ensure the total count of a pattern is odd.
/// </summary>
internal sealed class IsOdd : ISequenceConstraint
{
    /// <inheritdoc/>
    public bool Execute(SequenceBuilder sequence, Flex1d builder, double length, int count)
    {
        return (count + sequence._takeCountAdjustment + sequence._prependedItems.Count + sequence._appendedItems.Count) % 2 == 1;
    }
}
