namespace Geometrician.Core.Geometry;

/// <summary>
/// Methods to calculate rotations.
/// </summary>
public static class RotationHelpers
{
    /// <summary>
    /// Calculate the angle between <paramref name="this"/> and <paramref name="v"/> on the <paramref name="normal"/> axis.
    /// A rotation in the CCW direction is positive and CW direction are negative.
    /// </summary>
    public static double SignedPlaneAngleTo(this Vector3 @this, Vector3 v, Vector3 normal)
    {
        double angle = @this.PlaneAngleTo(v, normal);
        return angle <= 180
            ? angle
            : (360 - angle) * -1;
    }

    /// <summary>
    /// Calculate the angle between <paramref name="this"/> and <paramref name="v"/> on the <see cref="Vector3.ZAxis"/>
    /// A rotation in the CCW direction is positive and CW direction are negative.
    /// </summary>
    public static double SignedPlaneAngleTo(this Vector3 @this, Vector3 v)
    {
        return @this.SignedPlaneAngleTo(v, Vector3.ZAxis);
    }
}
