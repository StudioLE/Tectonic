namespace Lineweights.Flex;

public static class Conditions
{
    public static SequenceBuilder EvenCondition(this SequenceBuilder @this)
    {
        ISequenceBuilder.Condition constraint = (sequence, _) => sequence.Count % 2 == 0;
        @this.AddCondition(constraint);
        return @this;
    }

    public static SequenceBuilder OddCondition(this SequenceBuilder @this)
    {
        ISequenceBuilder.Condition constraint = (sequence, _) => sequence.Count % 2 == 1;
        @this.AddCondition(constraint);
        return @this;
    }
}
