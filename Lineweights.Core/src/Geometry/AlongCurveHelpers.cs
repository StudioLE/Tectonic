namespace Lineweights.Core.Geometry;

/// <summary>
/// Methods to find points and transforms along a curve.
/// </summary>
public static class AlongCurveHelpers
{
    /// <inheritdoc cref="Curve.TransformAt(double)"/>
    public static Transform TransformAtLength(this Curve @this, double length)
    {
        double u = length / @this.Length();
        Validate.OrThrow(new UParameterConstraint(u));
        return @this.TransformAt(u);
    }

    /// <inheritdoc cref="Curve.TransformAt(double)"/>
    public static Transform UnboundTransformAtLength(this Curve @this, double length)
    {
        double u = length / @this.Length();
        return @this switch
        {
            Line line => line.UnboundTransformAt(u),
            Arc arc => arc.UnboundTransformAt(u),
            _ => throw new ArgumentException($"{nameof(UnboundTransformAtLength)} only works for Line and Arc.")
        };
    }

    /// <inheritdoc cref="Line.PointAt(double)"/>
    public static Vector3 UnboundPointAt(this Line @this, double u)
    {
        double offset = @this.Length() * u;
        return @this.Start + offset * @this.Direction();
    }

    /// <inheritdoc cref="Arc.PointAt(double)"/>
    public static Vector3 UnboundPointAt(this Arc @this, double u)
    {

        var angle = @this.StartAngle + (@this.EndAngle - @this.StartAngle) * u;
        var theta = Units.DegreesToRadians(angle);
        var x = @this.Center.X + @this.Radius * Math.Cos(theta);
        var y = @this.Center.Y + @this.Radius * Math.Sin(theta);
        return new Vector3(x, y);
    }

    /// <inheritdoc cref="Line.TransformAt(double)"/>
    public static Transform UnboundTransformAt(this Line @this, double u)
    {
        return new(@this.UnboundPointAt(u), (@this.Start - @this.End).Unitized());
    }

    /// <inheritdoc cref="Arc.TransformAt(double)"/>
    public static Transform UnboundTransformAt(this Arc @this, double u)
    {
        var p = @this.UnboundPointAt(u);
        var x = (p - @this.Center).Unitized();
        var y = Vector3.ZAxis;
        return new(p, x, x.Cross(y));
    }
}
