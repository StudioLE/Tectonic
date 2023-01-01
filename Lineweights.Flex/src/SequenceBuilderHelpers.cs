namespace Lineweights.Flex;

internal static class SequenceBuilderHelpers
{
    public static IReadOnlyCollection<IReadOnlyCollection<Element>> BuildWithWrapping(this SequenceBuilder @this)
    {
        if (@this._appended.Any() || @this._prepended.Any())
            throw new($"{nameof(BuildWithWrapping)} doesn't support appended and prepended.");
        IReadOnlyCollection<Element> body = @this.Body;
        List<IReadOnlyCollection<Element>> output = new();
        while (body.Count > 0)
        {
            SequenceBuilder builder = new()
            {
                Body = body,
                _constraints = @this._constraints,
                _overflow = @this._overflow,
                _context = @this._context
            };

            IReadOnlyCollection<Element> sequence = builder.Build();
            output.Add(sequence);
            int taken = sequence.Count;
            body = body.Skip(taken).ToArray();
        }

        return output;
    }

    /// <summary>
    /// Split the sequence into multiple sequences.
    /// </summary>
    public static IReadOnlyCollection<SequenceBuilder> SplitWrapping(this SequenceBuilder @this)
    {
        if (!@this.Wrapping)
            return new[] { @this };
        IReadOnlyCollection<IReadOnlyCollection<Element>> sequences = @this.BuildWithWrapping();
        var builders = sequences
            .Select(sequence =>
            {
                SequenceBuilder builder = new()
                {
                    Body = sequence,
                    _constraints = @this._constraints,
                    _overflow = @this._overflow,
                    _context = @this._context
                };
                return builder;
            })
            .ToArray();
        return builders;
    }
}
