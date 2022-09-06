namespace Lineweights.Flex;

public class DefaultViewArrangement : Flex2d
{
    public DefaultViewArrangement()
    {
        Orientation(Vector3.XAxis, Vector3.YAxis.Negate(), Vector3.ZAxis);
        MainJustification(Justification.SpaceEvenly);
        CrossJustification(Justification.SpaceEvenly);
    }
}
