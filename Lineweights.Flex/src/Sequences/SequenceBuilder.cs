using StudioLE.Core.System;

namespace Lineweights.Flex.Sequences;

/// <summary>
/// Create a sequence of <see cref="Element"/> according to a set of conditions and constraints.
/// </summary>
/// <remarks>
/// This follows a <see href="https://refactoring.guru/design-patterns/builder">builder pattern</see>.
/// </remarks>
public class SequenceBuilder : ISequenceBuilder
{
    private const int MaxLoopCount = 10_000;
    private IReadOnlyCollection<Element> _body = Array.Empty<Element>();
    private IReadOnlyCollection<Element> _prepended = Array.Empty<Element>();
    private IReadOnlyCollection<Element> _appended = Array.Empty<Element>();
    private List<Constraints.Constraint> _constraints = new();
    private List<Conditions.Condition> _conditions = new();
    private bool _overflow;
    private object? _context;
    private bool _repetition = false;
    private bool _wrapping = false;

    #region Builder methods

    /// <inheritdoc/>
    public ISequenceBuilder Body(params Element[] elements)
    {
        _body = elements;
        return this;
    }

    /// <inheritdoc/>
    public ISequenceBuilder Prepend(params Element[] elements)
    {
        _prepended = elements;
        return this;
    }

    /// <inheritdoc/>
    public ISequenceBuilder Append(params Element[] elements)
    {
        _appended = elements;
        return this;
    }

    /// <inheritdoc/>
    public ISequenceBuilder AddConstraint(params Constraints.Constraint[] constraints)
    {
        _constraints.AddRange(constraints);
        return this;
    }

    /// <inheritdoc/>
    public ISequenceBuilder AddCondition(params Conditions.Condition[] conditions)
    {
        _conditions.AddRange(conditions);
        return this;
    }

    /// <inheritdoc/>
    public ISequenceBuilder Overflow(bool overflow)
    {
        _overflow = overflow;
        return this;
    }

    /// <inheritdoc/>
    public ISequenceBuilder Context(object context)
    {
        _context = context;
        return this;
    }

    /// <inheritdoc/>
    public ISequenceBuilder Wrapping(bool wrapping)
    {
        _wrapping = wrapping;
        return this;
    }

    /// <inheritdoc/>
    public ISequenceBuilder Repetition(bool repetition)
    {
        _repetition = repetition;
        return this;
    }

    #endregion

    #region Execution methods

    /// <summary>
    /// Build the sequence.
    /// </summary>
    public IReadOnlyCollection<Element> Build()
    {
        if (_context is null)
            throw new("Build was called before the context was set");
        if (_wrapping)
            throw new($"Building wrapping sequences is not supported. Use {nameof(BuildWithWrapping)} instead.");
        return BuildWithoutWrapping();
    }

    /// <summary>
    /// Build the sequence.
    /// </summary>
    private IReadOnlyCollection<Element> BuildWithoutWrapping()
    {
        if (_context is null)
            throw new("Build was called before the context was set");
        IReadOnlyCollection<Element> output = Array.Empty<Element>();
        int maxCount = _body.Count + _appended.Count + _prepended.Count;
        int taken = 0;
        bool finalLoop = false;
        while (true)
        {
            if (taken > MaxLoopCount)
                break;
            if (!_repetition && output.Count >= maxCount)
                break;
            IReadOnlyCollection<Element> repetition = Array.Empty<Element>();
            int take = taken;
            int excess = taken - _body.Count;
            if (_repetition && excess > 0)
            {
                int complete = (excess / (double)_body.Count).FloorToInt();
                take = excess % _body.Count;
                repetition = Enumerable.Repeat(_body, complete).SelectMany(x => x).ToArray();
            }
            IEnumerable<Element> body = _body.Take(take);
            IReadOnlyCollection<Element> sequence = Enumerable.Empty<Element>()
                .Concat(_prepended)
                .Concat(repetition)
                .Concat(body)
                .Concat(_appended)
                .ToArray();
            if (finalLoop || !ValidateConstraints(sequence, _context))
            {
                if (!finalLoop && _overflow)
                    finalLoop = true;
                else
                    break;
            }
            taken++;
            if (_conditions.Any() && !ValidateConditions(sequence, _context))
                continue;
            output = sequence;
        }
        return output;
    }

    /// <summary>
    /// Split the sequence into multiple sequences.
    /// </summary>
    private IReadOnlyCollection<IReadOnlyCollection<Element>> BuildWithWrapping()
    {
        if (_appended.Any() || _prepended.Any())
            throw new($"{nameof(BuildWithWrapping)} doesn't support appended and prepended.");
        IReadOnlyCollection<Element> body = _body;
        List<IReadOnlyCollection<Element>> output = new();
        while (body.Count > 0)
        {
            SequenceBuilder builder = new()
            {
                _body = body,
                _constraints = _constraints,
                _overflow = _overflow,
                _context = _context,
                _wrapping = false
            };

            IReadOnlyCollection<Element> sequence = builder.BuildWithoutWrapping();
            output.Add(sequence);
            int taken = sequence.Count;
            body = body.Skip(taken).ToArray();
        }

        return output;
    }

    /// <summary>
    /// Split the sequence into multiple sequences.
    /// </summary>
    public IReadOnlyCollection<ISequenceBuilder> SplitWrapping()
    {
        if (!_wrapping)
            return new[] { this };
        IReadOnlyCollection<IReadOnlyCollection<Element>> sequences = BuildWithWrapping();
        var builders = sequences
            .Select(sequence =>
            {
                SequenceBuilder builder = new()
                {
                    _body = sequence,
                    _constraints = _constraints,
                    _overflow = _overflow,
                    _context = _context
                };
                return builder;
            })
            .ToArray();
        return builders;
    }

    private bool ValidateConstraints(IReadOnlyCollection<Element> sequence, object  context)
    {
        bool isValid = _constraints.All(constraint => constraint.Invoke(sequence, context));
        return isValid;
    }

    private bool ValidateConditions(IReadOnlyCollection<Element> sequence, object  context)
    {

        bool isValid = _conditions.All(condition => condition.Invoke(sequence, context));
        return isValid;
    }

    #endregion
}
