namespace Lineweights.Flex.Sequences;

public static class Conditions
{
    public delegate bool Condition(IReadOnlyCollection<Element> sequence, object context);

    public static ISequenceBuilder EvenCondition(this ISequenceBuilder @this)
    {
        Condition constraint = (sequence, context) => sequence.Count % 2 == 0;
        @this.AddCondition(constraint);
        return @this;
    }

    public static ISequenceBuilder OddCondition(this ISequenceBuilder @this)
    {
        Condition constraint = (sequence, context) => sequence.Count % 2 == 1;
        @this.AddCondition(constraint);
        return @this;
    }
}
