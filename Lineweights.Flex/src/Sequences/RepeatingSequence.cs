namespace Lineweights.Flex.Sequences;

/// <summary>
/// A sequence of elements that repeats.
/// </summary>
public sealed class RepeatingSequence : SequenceBuilder
{
    private RepeatingSequence()
    {
    }

    /// <summary>
    /// A sequence of elements that repeats and overflows its container by one.
    /// </summary>
    public static SequenceBuilder WithOverflow(params ElementInstance[] repeatingItems)
    {
        return new RepeatingSequence()
            .Items(repeatingItems)
            .AddConstraint(new MaxLength())
            .AdjustTakeCount(1);
    }

    /// <summary>
    /// A sequence of elements that repeats and does not overflow its container.
    /// </summary>
    public static SequenceBuilder WithoutOverflow(params ElementInstance[] repeatingItems)
    {
        return new RepeatingSequence()
            .Items(repeatingItems)
            .AddConstraint(new MaxLength());
    }

    /// <summary>
    /// A sequence of elements that repeats a maximum number of times.
    /// </summary>
    public static SequenceBuilder MaxCount(int count, params ElementInstance[] repeatingItems)
    {
        return new RepeatingSequence()
            .Items(repeatingItems)
            .AddConstraint(new MaxCount(count));
    }
}
