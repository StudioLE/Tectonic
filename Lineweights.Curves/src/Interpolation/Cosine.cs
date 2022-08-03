namespace Lineweights.Curves.Interpolation;

/// <inheritdoc cref="IInterpolation"/>
public sealed class Cosine : IInterpolation
{
    /// <inheritdoc/>
    public double Interpolate(
        double mu,
        double startVector,
        double endVector,
        double? previousVector = null,
        double? nextVector = null)
    {
        double mu2 = (1 - Math.Cos(mu * Math.PI)) / 2;
        return startVector * (1 - mu2) + endVector * mu2;
    }
}
