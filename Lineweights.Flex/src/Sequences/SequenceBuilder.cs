using StudioLE.Core.Collections;

namespace Lineweights.Flex.Sequences;

/// <summary>
/// Create a pattern of <see cref="ElementInstance"/> according to a set of constraints.
/// </summary>
/// <remarks>
/// This follows a <see href="https://refactoring.guru/design-patterns/builder">builder pattern</see>.
/// </remarks>
public abstract class SequenceBuilder
{
    #region Fields

    /// <summary>
    /// The index to start the sequence at.
    /// </summary>
    internal int _startIndex = 0;

    /// <summary>
    /// Adjust the take count.
    /// </summary>
    internal int _takeCountAdjustment = 0;

    /// <summary>
    /// The repeating items in the sequence.
    /// </summary>
    internal IReadOnlyCollection<Proxy> _items = Array.Empty<Proxy>();

    /// <summary>
    /// The items to place before the repeating items.
    /// </summary>
    internal IReadOnlyCollection<Proxy> _prependedItems = Array.Empty<Proxy>();

    /// <summary>
    /// The items to place after the repeating items.
    /// </summary>
    internal IReadOnlyCollection<Proxy> _appendedItems = Array.Empty<Proxy>();

    /// <summary>
    /// The constraints that limit the sequence.
    /// </summary>
    internal IReadOnlyCollection<ISequenceConstraint> _constraints = Array.Empty<ISequenceConstraint>();

    /// <inheritdoc cref="Sequences.ConstraintMode"/>
    internal ConstraintMode _mode = Sequences.ConstraintMode.And;

    #endregion

    /// <inheritdoc cref="SequenceBuilder"/>
    protected SequenceBuilder()
    {
    }

    /// <inheritdoc cref="SequenceBuilder"/>
    protected SequenceBuilder(SequenceBuilder prototype)
    {
        _startIndex = prototype._startIndex;
        _takeCountAdjustment = prototype._takeCountAdjustment;
        _items = prototype._items;
        _prependedItems = prototype._prependedItems;
        _appendedItems = prototype._appendedItems;
        _constraints = prototype._constraints;
    }

    #region Builder methods


    /// <inheritdoc cref="_takeCountAdjustment"/>
    internal SequenceBuilder AdjustTakeCount(int adjustment)
    {
        _takeCountAdjustment += adjustment;
        return this;
    }

    /// <inheritdoc cref="_items"/>
    internal SequenceBuilder Items(params ElementInstance[] elements)
    {
        _items = elements.Select(x => new Proxy(x)).ToArray();
        return this;
    }

    /// <inheritdoc cref="_items"/>
    internal SequenceBuilder Items(params Proxy[] elements)
    {
        _items = elements.Select(x => new Proxy(x)).ToArray();
        return this;
    }

    /// <inheritdoc cref="_prependedItems"/>
    internal SequenceBuilder PrependedItems(params ElementInstance[] elements)
    {
        _prependedItems = elements.Select(x => new Proxy(x)).ToArray();
        return this;
    }

    /// <inheritdoc cref="_appendedItems"/>
    internal SequenceBuilder AppendedItems(params ElementInstance[] elements)
    {
        _appendedItems = elements.Select(x => new Proxy(x)).ToArray();
        return this;
    }

    /// <inheritdoc cref="_constraints"/>
    internal SequenceBuilder AddConstraint(params ISequenceConstraint[] constraints)
    {
        _constraints = _constraints.Concat(constraints).ToArray();
        return this;
    }

    /// <inheritdoc cref="_mode"/>
    internal SequenceBuilder ConstraintMode(ConstraintMode mode)
    {
        _mode = mode;
        return this;
    }

    #endregion

    #region Execution methods

    /// <summary>
    /// Build the sequence.
    /// </summary>
    internal IEnumerable<Proxy> Build(Flex1d builder)
    {
        int count = TakeCount(builder);
        IEnumerable<Proxy> repeatItems = Enumerable
            .Range(_startIndex, count)
            .Select(i => new Proxy(_items.ElementAtWithWrapping(i))); // TODO: Does this ignore the first element?
        return _prependedItems.Concat(repeatItems).Concat(_appendedItems);
    }

    /// <summary>
    /// Determine the number of repeating items to take.
    /// </summary>
    protected int TakeCount(Flex1d builder)
    {
        List<Proxy> repeatedItems = new();
        double length = 0;
        int count = 0;
        int index = _startIndex;
        while (_mode == Sequences.ConstraintMode.And && _constraints.All(x => x.Execute(this, builder, length, count))
               || _mode == Sequences.ConstraintMode.Or && _constraints.Any(x => x.Execute(this, builder, length, count)))
        {
            Proxy next = new(_items.ElementAtWithWrapping(index));
            repeatedItems.Add(next);
            length = builder.MainDimensionWithMinSpacing(_prependedItems.Concat(repeatedItems).Concat(_appendedItems).ToArray());
            count++;
            index++;
        }

        return count - 1 + _takeCountAdjustment;
    }

    #endregion
}
