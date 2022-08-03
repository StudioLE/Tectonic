using StudioLE.Core.System;

namespace Lineweights.Core.Geometry;

/// <summary>
/// Methods to round geometry.
/// </summary>
public static class RoundingHelpers
{
    /// <summary>
    /// Round the axis values of the <see cref="Transform"/>.
    /// </summary>
    public static Transform RoundedAxis(this Transform @this, int decimalPlaces)
    {
        return new(@this.Origin, @this.XAxis.Round(decimalPlaces), @this.YAxis.Round(decimalPlaces), @this.ZAxis.Round(decimalPlaces));
    }

    /// <summary>
    /// Round the value of the <see cref="Vector3"/>.
    /// </summary>
    public static Vector3 Round(this Vector3 @this, int decimalPlaces)
    {
        return new(@this.X.Round(decimalPlaces), @this.Y.Round(decimalPlaces), @this.Z.Round(decimalPlaces));
    }
}
