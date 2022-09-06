namespace Lineweights.Core.Abstractions;

public interface ISequenceBuilder
{
    /// <summary>
    /// Set the elements in the body of the sequence.
    /// For repeating sequences it is the body items that are repeated.
    /// </summary>
    ISequenceBuilder Body(params Element[] elements);

    /// <summary>
    /// Set the elements to include before the body.
    /// </summary>
    ISequenceBuilder Prepend(params Element[] elements);

    /// <summary>
    /// Set the elements to include after the body.
    /// </summary>
    ISequenceBuilder Append(params Element[] elements);

    /// <summary>
    /// Add a constraint.
    /// </summary>
    ISequenceBuilder AddConstraint(params Constraint[] constraints);

    /// <summary>
    /// Add a condition.
    /// If a condition returns false then the build loop skips to the next iteration.
    /// </summary>
    ISequenceBuilder AddCondition(params Condition[] conditions);

    /// <summary>
    /// When enabled the build loops runs once more after a constraint is met.
    /// </summary>
    ISequenceBuilder Overflow(bool overflow);

    /// <summary>
    /// Set the context that is passed to conditions and constraints.
    /// </summary>
    ISequenceBuilder Context(object context);

    /// <summary>
    /// When enabled the sequence can be split into multiple lines via the <see cref="SplitWrapping"/> method.
    /// </summary>
    ISequenceBuilder Wrapping(bool wrapping);

    /// <summary>
    /// Should the body elements be repeated?
    /// </summary>
    ISequenceBuilder Repetition(bool repetition);

    /// <summary>
    /// Build the sequence.
    /// </summary>
    IReadOnlyCollection<Element> Build();

    /// <summary>
    /// Split the sequence into multiple sequences that have been wrapped.
    /// </summary>
    IReadOnlyCollection<ISequenceBuilder> SplitWrapping();

    /// <summary>
    /// If a condition returns false then the build loop skips to the next iteration.
    /// </summary>
    public delegate bool Condition(IReadOnlyCollection<Element> sequence, object context);


    /// <summary>
    /// Once a constraint returns false the build loop ends and the sequence is returned.
    /// </summary>
    public delegate bool Constraint(IReadOnlyCollection<Element> sequence, object context);
}
