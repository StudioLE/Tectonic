namespace Lineweights.Flex.Sequences;

/// <summary>
/// A pattern which wraps to a new line when it meets its constraint.
/// </summary>
public sealed class WrappingSequence : SequenceBuilder
{
    private WrappingSequence()
    {
    }

    private WrappingSequence(WrappingSequence prototype) : base(prototype)
    {
    }

    /// <summary>
    /// Split the pattern to a new line.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public IEnumerable<SequenceBuilder> Split(Flex1d builder)
    {
        List<SequenceBuilder> patterns = new();
        int initialIndex = _startIndex;
        while (_startIndex < _items.Count)
        {
            int count = TakeCount(builder);
            Proxy[] items = _items.Skip(_startIndex).Take(count).ToArray();
            patterns.Add(NonRepeatingSequence.WithoutOverflow(items));
            _startIndex += count;
        }
        _startIndex = initialIndex;
        return patterns;
    }
    
    /// <summary>
    /// A pattern which wraps to a new line when it meets its constraint.
    /// </summary>
    public static SequenceBuilder WithoutOverflow(params ElementInstance[] items)
    {
        return new WrappingSequence()
            .Items(items)
            .AddConstraint(new MaxLength())
            .AddConstraint(new NonRepeating());
    }
}
