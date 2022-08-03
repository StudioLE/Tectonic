namespace Lineweights.Curves.Interpolation;

/// <inheritdoc cref="IInterpolation"/>
public sealed class Linear : IInterpolation
{
    /// <inheritdoc/>
    public double Interpolate(
        double mu,
        double startVector,
        double endVector,
        double? previousVector = null,
        double? nextVector = null)
    {
        return startVector * (1 - mu) + endVector * mu;
    }
}
