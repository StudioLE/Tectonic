namespace Geometrician.Curves.Interpolation;

/// <inheritdoc cref="IInterpolation"/>
public sealed class CatmullRom : IInterpolation
{
    /// <inheritdoc/>
    public double Interpolate(
        double mu,
        double startVector,
        double endVector,
        double? previousVector = null,
        double? nextVector = null)
    {
        if (previousVector is null)
            throw new ArgumentException($"Failed to interpolate {nameof(CatmullRom)}. {nameof(previousVector)} must be set.", nameof(previousVector));
        if (nextVector is null)
            throw new ArgumentException($"Failed to interpolate {nameof(CatmullRom)}. {nameof(nextVector)} must be set.", nameof(nextVector));

        double mu2 = mu * mu;
        double a0 = -0.5 * (double)previousVector + 1.5 * startVector - 1.5 * endVector + 0.5 * (double)nextVector;
        double a1 = (double)previousVector - 2.5 * startVector + 2 * endVector - 0.5 * (double)nextVector;
        double a2 = -0.5 * (double)previousVector + 0.5 * endVector;
        double a3 = startVector;

        return a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3;
    }
}
