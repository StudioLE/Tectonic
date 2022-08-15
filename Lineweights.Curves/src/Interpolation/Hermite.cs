namespace Lineweights.Curves.Interpolation;

/// <inheritdoc cref="IInterpolation"/>
public sealed class Hermite : IInterpolation
{
    /// <summary>
    /// The tension.
    /// 1 is high, 0 normal, -1 is low.
    /// </summary>
    public double Tension { get; set; }

    /// <summary>
    /// The bias.
    /// 0 is even,
    /// positive is towards first segment,
    /// negative towards the other.
    /// </summary>
    public double Bias { get; set; }

    /// <inheritdoc/>
    public double Interpolate(
        double mu,
        double startVector,
        double endVector,
        double? previousVector = null,
        double? nextVector = null)
    {
        if (previousVector is null)
            throw new ArgumentException($"Failed to interpolate {nameof(Hermite)}. {nameof(previousVector)} must be set.", nameof(previousVector));
        if (nextVector is null)
            throw new ArgumentException($"Failed to interpolate {nameof(Hermite)}. {nameof(nextVector)} must be set.", nameof(nextVector));

        double mu2 = mu * mu;
        double mu3 = mu2 * mu;
        double m0 = (startVector - (double)previousVector) * (1 + Bias) * (1 - Tension) / 2;
        m0 += (endVector - startVector) * (1 - Bias) * (1 - Tension) / 2;
        double m1 = (endVector - startVector) * (1 + Bias) * (1 - Tension) / 2;
        m1 += ((double)nextVector - endVector) * (1 - Bias) * (1 - Tension) / 2;
        double a0 = 2 * mu3 - 3 * mu2 + 1;
        double a1 = mu3 - 2 * mu2 + mu;
        double a2 = mu3 - mu2;
        double a3 = -2 * mu3 + 3 * mu2;

        return a0 * startVector + a1 * m0 + a2 * m1 + a3 * endVector;
    }
}
