namespace StudioLE.Core.System;

/// <summary>
/// Methods to help with <see cref="Math"/>.
/// </summary>
public static class MathHelpers
{
    /// <summary>
    /// Return an enumerable which is the cumulative sum of each proceeding value in <paramref name="sequence"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://stackoverflow.com/a/4831908/247218">Source</see>
    /// </remarks>
    public static IEnumerable<double> CumulativeSum(this IEnumerable<double> sequence)
    {
        double sum = 0;
        foreach (double item in sequence)
        {
            sum += item;
            yield return sum;
        }
    }

    /// <summary>
    /// Round <paramref name="this"/> to <paramref name="decimalPlaces"/> using <see cref="MidpointRounding.AwayFromZero"/>.
    /// </summary>
    public static double Round(this double @this, int decimalPlaces)
    {
        double result = Math.Round(@this, decimalPlaces, MidpointRounding.AwayFromZero);
        return result == 0
            ? 0
            : result;
    }

    /// <summary>
    /// Round <paramref name="this"/> to an integer using <see cref="MidpointRounding.AwayFromZero"/>.
    /// </summary>
    public static int RoundToInt(this double @this)
    {
        return (int)Math.Round(@this, 0, MidpointRounding.AwayFromZero);
    }

    /// <inheritdoc cref="Math.Ceiling(double)"/>
    public static int CeilingToInt(this double @this)
    {
        return (int)Math.Ceiling(@this);
    }

    /// <inheritdoc cref="Math.Floor(double)"/>
    public static int FloorToInt(this double @this)
    {
        return (int)Math.Floor(@this);
    }
}
