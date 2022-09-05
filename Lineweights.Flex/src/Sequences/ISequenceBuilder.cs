namespace Lineweights.Flex.Sequences;

public interface ISequenceBuilder
{
    /// <inheritdoc cref="SequenceBuilder._body"/>
    ISequenceBuilder Body(params Element[] elements);

    /// <inheritdoc cref="SequenceBuilder._appended"/>
    ISequenceBuilder Prepend(params Element[] elements);

    /// <inheritdoc cref="SequenceBuilder._appended"/>
    ISequenceBuilder Append(params Element[] elements);

    /// <inheritdoc cref="SequenceBuilder._constraints"/>
    ISequenceBuilder AddConstraint(params Constraints.Constraint[] constraints);

    ISequenceBuilder AddCondition(params Conditions.Condition[] conditions);

    /// <inheritdoc cref="SequenceBuilder._overflow"/>
    ISequenceBuilder Overflow(bool overflow);

    /// <inheritdoc cref="SequenceBuilder._overflow"/>
    ISequenceBuilder Context(object context);

    /// <inheritdoc cref="SequenceBuilder._overflow"/>
    ISequenceBuilder Wrapping(bool wrapping);

    ISequenceBuilder Repetition(bool repetition);

    /// <summary>
    /// Build the sequence.
    /// </summary>
    IReadOnlyCollection<Element> Build();

    IReadOnlyCollection<ISequenceBuilder> SplitWrapping();
}
