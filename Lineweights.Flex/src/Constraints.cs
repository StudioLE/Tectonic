namespace Lineweights.Flex;

public static class Constraints
{
    public static ISequenceBuilder MaxLengthConstraint(this ISequenceBuilder @this)
    {
        ISequenceBuilder.Constraint constraint = (sequence, context) =>
        {
            if (context is not Flex1d flex)
                throw new($"Failed to create constraint. Expected context to be a {nameof(Flex1d)}");
            Proxy[] proxies = sequence.Select(Proxy.Create).ToArray();
            double totalLength = flex.MainDimensionWithMinSpacing(proxies);
            return totalLength < flex.TargetLength;
        };
        @this.AddConstraint(constraint);
        return @this;
    }

    public static ISequenceBuilder MaxCountConstraint(this ISequenceBuilder @this, int maxCount)
    {
        ISequenceBuilder.Constraint constraint = (sequence, context) => sequence.Count <= maxCount;
        @this.AddConstraint(constraint);
        return @this;
    }
}
