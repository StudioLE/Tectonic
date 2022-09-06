namespace Lineweights.Flex;

public static class Conditions
{
    public static ISequenceBuilder EvenCondition(this ISequenceBuilder @this)
    {
        ISequenceBuilder.Condition constraint = (sequence, context) => sequence.Count % 2 == 0;
        @this.AddCondition(constraint);
        return @this;
    }

    public static ISequenceBuilder OddCondition(this ISequenceBuilder @this)
    {
        ISequenceBuilder.Condition constraint = (sequence, context) => sequence.Count % 2 == 1;
        @this.AddCondition(constraint);
        return @this;
    }
}
