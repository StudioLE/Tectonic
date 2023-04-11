namespace Geometrician.Curves.Interpolation;

/// <summary>
/// Methods to interpolate a <see cref="Spline"/> ensuring that the curve passes through each of
/// the <see cref="Spline.KeyVertices"/>.
/// </summary>
/// <remarks>
/// <see href="http://paulbourke.net/miscellaneous/interpolation/">More information</see>.
/// </remarks>
public interface IInterpolation
{
    /// <inheritdoc cref="IInterpolation"/>
    double Interpolate(
        double mu,
        double startVector,
        double endVector,
        double? previousVector = null,
        double? nextVector = null);
}
