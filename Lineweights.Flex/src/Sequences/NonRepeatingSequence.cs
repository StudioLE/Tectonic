namespace Lineweights.Flex.Sequences;

/// <summary>
/// A sequence of elements that does not repeat.
/// </summary>
public sealed class NonRepeatingSequence : SequenceBuilder
{
    private NonRepeatingSequence()
    {
    }

    /// <summary>
    /// A sequence of elements that does not repeat and does not overflow its container.
    /// </summary>
    internal static SequenceBuilder WithoutOverflow(IEnumerable<Proxy> items)
    {
        return new NonRepeatingSequence()
            .Items(items.ToArray())
            .AddConstraint(new MaxLength())
            .AddConstraint(new NonRepeating());
    }

    /// <summary>
    /// A sequence of elements that does not repeat and does not overflow its container.
    /// </summary>
    public static SequenceBuilder WithoutOverflow(params ElementInstance[] items)
    {
        return new NonRepeatingSequence()
            .Items(items)
            .AddConstraint(new MaxLength())
            .AddConstraint(new NonRepeating());
    }

    // TODO: NonRepeatingPattern.WithOverflow?
}
