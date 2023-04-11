namespace Geometrician.Curves.Interpolation;

/// <inheritdoc cref="IInterpolation"/>
public sealed class Cubic : IInterpolation
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
            throw new ArgumentException($"Failed to interpolate {nameof(Cubic)}. {nameof(previousVector)} must be set.", nameof(previousVector));
        if (nextVector is null)
            throw new ArgumentException($"Failed to interpolate {nameof(Cubic)}. {nameof(nextVector)} must be set.", nameof(nextVector));

        double mu2 = mu * mu;
        double a0 = (double)nextVector - endVector - (double)previousVector + startVector;
        double a1 = (double)previousVector - startVector - a0;
        double a2 = endVector - (double)previousVector;
        double a3 = startVector;

        return a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3;
    }
}
