using StudioLE.Core.System;

namespace Lineweights.Flex;

/// <summary>
/// Create a sequence of <see cref="Element"/> according to a set of conditions and constraints.
/// </summary>
/// <remarks>
/// This follows a <see href="https://refactoring.guru/design-patterns/builder">builder pattern</see>.
/// </remarks>
public class SequenceBuilder : ISequenceBuilder
{
    internal const int MaxLoopCount = 10_000;
    internal IReadOnlyCollection<Element> _prepended = Array.Empty<Element>();
    internal IReadOnlyCollection<Element> _appended = Array.Empty<Element>();
    internal List<ISequenceBuilder.Constraint> _constraints = new();
    internal List<ISequenceBuilder.Condition> _conditions = new();
    internal bool _overflow;
    internal object? _context;
    internal bool _repetition;

    public IReadOnlyCollection<Element> Body { get; set; } = Array.Empty<Element>();

    public bool Wrapping { get; set; } = false;

    #region Builder methods

    public SequenceBuilder SetBody(params Element[] elements)
    {
        Body = elements;
        return this;
    }

    public SequenceBuilder Prepend(params Element[] elements)
    {
        _prepended = elements;
        return this;
    }

    public SequenceBuilder Append(params Element[] elements)
    {
        _appended = elements;
        return this;
    }

    public SequenceBuilder AddConstraint(params ISequenceBuilder.Constraint[] constraints)
    {
        _constraints.AddRange(constraints);
        return this;
    }

    public SequenceBuilder AddCondition(params ISequenceBuilder.Condition[] conditions)
    {
        _conditions.AddRange(conditions);
        return this;
    }

    public SequenceBuilder Overflow(bool overflow)
    {
        _overflow = overflow;
        return this;
    }

    public SequenceBuilder Context(object context)
    {
        _context = context;
        return this;
    }

    public SequenceBuilder Repetition(bool repetition)
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
        if (Wrapping)
            throw new($"Building wrapping sequences is not supported. Use {nameof(SequenceBuilderHelpers.BuildWithWrapping)} instead.");
        IReadOnlyCollection<Element> output = Array.Empty<Element>();
        int maxCount = Body.Count + _appended.Count + _prepended.Count;
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
            int excess = taken - Body.Count;
            if (_repetition && excess > 0)
            {
                int complete = (excess / (double)Body.Count).FloorToInt();
                take = excess % Body.Count;
                repetition = Enumerable.Repeat(Body, complete).SelectMany(x => x).ToArray();
            }
            IEnumerable<Element> body = Body.Take(take);
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

    private bool ValidateConstraints(IReadOnlyCollection<Element> sequence, object context)
    {
        bool isValid = _constraints.All(constraint => constraint.Invoke(sequence, context));
        return isValid;
    }

    private bool ValidateConditions(IReadOnlyCollection<Element> sequence, object context)
    {
        bool isValid = _conditions.All(condition => condition.Invoke(sequence, context));
        return isValid;
    }

    #endregion
}
