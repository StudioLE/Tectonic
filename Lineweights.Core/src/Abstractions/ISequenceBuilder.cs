namespace Lineweights.Core.Abstractions;

public interface ISequenceBuilder
{
    // /// <summary>
    // /// Set the elements in the body of the sequence.
    // /// For repeating sequences it is the body items that are repeated.
    // /// </summary>
    public IReadOnlyCollection<Element> Body { set; }

    public bool Wrapping { set; }

    /// <summary>
    /// If a condition returns false then the build loop skips to the next iteration.
    /// </summary>
    public delegate bool Condition(IReadOnlyCollection<Element> sequence, object context);


    /// <summary>
    /// Once a constraint returns false the build loop ends and the sequence is returned.
    /// </summary>
    public delegate bool Constraint(IReadOnlyCollection<Element> sequence, object context);
}
