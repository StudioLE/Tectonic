namespace Lineweights.Core.Geometry;

/// <summary>
/// Methods to calculate rotations.
/// </summary>
public static class TransformHelpers
{
    /// <summary>
    /// Calculate the rotation from <paramref name="source"/> to match the <paramref name="target"/>.
    /// </summary>
    public static Transform RotationBetween(Transform source, Transform? target = null)
    {
        target ??= new();
        Transform comparison = new(source);
        Transform rotation = new();

        // X rotation
        double rotationXAroundX = target.XAxis.SignedPlaneAngleTo(comparison.XAxis, target.XAxis);
        if (!double.IsNaN(rotationXAroundX))
        {
            rotation.Rotate(target.XAxis, rotationXAroundX);
            comparison = source.Concatenated(rotation);
        }

        double rotationYAroundX = target.YAxis.SignedPlaneAngleTo(comparison.YAxis, target.XAxis);
        if (!double.IsNaN(rotationYAroundX))
        {
            rotation.Rotate(target.XAxis, rotationYAroundX);
            comparison = source.Concatenated(rotation);
        }

        // Y rotation
        double rotationXAroundY = target.XAxis.SignedPlaneAngleTo(comparison.XAxis, target.YAxis);
        if (!double.IsNaN(rotationXAroundY))
        {
            rotation.Rotate(target.YAxis, rotationXAroundY);
            comparison = source.Concatenated(rotation);
        }

        double rotationYAroundY = target.YAxis.SignedPlaneAngleTo(comparison.YAxis, target.YAxis);
        if (!double.IsNaN(rotationYAroundY))
        {
            rotation.Rotate(target.YAxis, rotationYAroundY);
            comparison = source.Concatenated(rotation);
        }

        // Z rotation
        double rotationXAroundZ = target.XAxis.SignedPlaneAngleTo(comparison.XAxis, target.ZAxis);
        if (!double.IsNaN(rotationXAroundZ))
        {
            rotation.Rotate(target.ZAxis, rotationXAroundZ);
            comparison = source.Concatenated(rotation);
        }

        double rotationYAroundZ = target.YAxis.SignedPlaneAngleTo(comparison.YAxis, target.ZAxis);
        if (!double.IsNaN(rotationYAroundZ))
        {
            rotation.Rotate(target.ZAxis, rotationYAroundZ);
            comparison = source.Concatenated(rotation);
        }

#if false
        double checkXAroundX = target.XAxis.SignedPlaneAngleTo(comparison.XAxis, target.XAxis);
        double checkXAroundY = target.XAxis.SignedPlaneAngleTo(comparison.XAxis, target.YAxis);
        double checkXAroundZ = target.XAxis.SignedPlaneAngleTo(comparison.XAxis, target.ZAxis);

        double checkYAroundX = target.YAxis.SignedPlaneAngleTo(comparison.YAxis, target.XAxis);
        double checkYAroundY = target.YAxis.SignedPlaneAngleTo(comparison.YAxis, target.YAxis);
        double checkYAroundZ = target.YAxis.SignedPlaneAngleTo(comparison.YAxis, target.ZAxis);

        double checkZAroundX = target.ZAxis.SignedPlaneAngleTo(comparison.ZAxis, target.XAxis);
        double checkZAroundY = target.ZAxis.SignedPlaneAngleTo(comparison.ZAxis, target.YAxis);
        double checkZAroundZ = target.ZAxis.SignedPlaneAngleTo(comparison.ZAxis, target.ZAxis);
#endif

        return rotation;
    }

    /// <summary>
    /// Rotate the <paramref name="source"/> to match the <paramref name="target"/>.
    /// </summary>
    public static void RotateTo(this Transform source, Transform target)
    {
        Vector3 translation = Vector3.Origin - source.Origin;
        source.Move(translation);

        // X rotation
        double rotationXAroundX = target.XAxis.SignedPlaneAngleTo(source.XAxis, target.XAxis);
        if (!double.IsNaN(rotationXAroundX))
            source.Rotate(target.XAxis, rotationXAroundX * -1);

        double rotationYAroundX = target.YAxis.SignedPlaneAngleTo(source.YAxis, target.XAxis);
        if (!double.IsNaN(rotationYAroundX))
            source.Rotate(target.XAxis, rotationYAroundX * -1);

        // Y rotation
        double rotationXAroundY = target.XAxis.SignedPlaneAngleTo(source.XAxis, target.YAxis);
        if (!double.IsNaN(rotationXAroundY))
            source.Rotate(target.YAxis, rotationXAroundY * -1);

        double rotationYAroundY = target.YAxis.SignedPlaneAngleTo(source.YAxis, target.YAxis);
        if (!double.IsNaN(rotationYAroundY))
            source.Rotate(target.YAxis, rotationYAroundY * -1);

        // Z rotation
        double rotationXAroundZ = target.XAxis.SignedPlaneAngleTo(source.XAxis, target.ZAxis);
        if (!double.IsNaN(rotationXAroundZ))
            source.Rotate(target.ZAxis, rotationXAroundZ * -1);

        double rotationYAroundZ = target.YAxis.SignedPlaneAngleTo(source.YAxis, target.ZAxis);
        if (!double.IsNaN(rotationYAroundZ))
            source.Rotate(target.ZAxis, rotationYAroundZ * -1);

#if false
        double checkXAroundX = target.XAxis.SignedPlaneAngleTo(result.XAxis, target.XAxis);
        double checkXAroundY = target.XAxis.SignedPlaneAngleTo(result.XAxis, target.YAxis);
        double checkXAroundZ = target.XAxis.SignedPlaneAngleTo(result.XAxis, target.ZAxis);

        double checkYAroundX = target.YAxis.SignedPlaneAngleTo(result.YAxis, target.XAxis);
        double checkYAroundY = target.YAxis.SignedPlaneAngleTo(result.YAxis, target.YAxis);
        double checkYAroundZ = target.YAxis.SignedPlaneAngleTo(result.YAxis, target.ZAxis);

        double checkZAroundX = target.ZAxis.SignedPlaneAngleTo(result.ZAxis, target.XAxis);
        double checkZAroundY = target.ZAxis.SignedPlaneAngleTo(result.ZAxis, target.YAxis);
        double checkZAroundZ = target.ZAxis.SignedPlaneAngleTo(result.ZAxis, target.ZAxis);
#endif

        source.Move(translation.Negate());
    }
}
