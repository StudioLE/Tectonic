using StudioLE.Core.Exceptions;

namespace Lineweights.PDF.From.Geometry;

/// <summary>
/// Convert a <see cref="Curve"/> to a <see cref="SKPath"/>.
/// </summary>
internal sealed class CurveToPdf : IConverter<Curve, SKPath>
{
    /// <inheritdoc cref="CurveToPdf" />
    public SKPath Convert(Curve curve)
    {
        return curve switch
        {
            //Circle circle => throw new NotImplementedException(),
            //Arc arc => throw new NotImplementedException(),
            //Bezier bezier => throw new NotImplementedException(),
            Line line => new LineToPdf().Convert(line),
            Polyline polyline => new PolylineToPdf().Convert(polyline),
            _ => throw new TypeSwitchException<Curve>("Failed to convert curve to PDF.", curve)
        };
    }
}

/// <summary>
/// Convert a <see cref="Line"/> to a <see cref="SKPath"/>.
/// </summary>
internal sealed class LineToPdf : IConverter<Line, SKPath>
{
    /// <inheritdoc cref="LineToPdf" />
    public SKPath Convert(Line line)
    {
        Vector3ToPdf converter = new();
        SKPath path = new();
        path.AddPoly(new[] { line.Start, line.End }.Select(converter.Convert).ToArray());
        return path;
    }
}

/// <summary>
/// Convert a <see cref="Polyline"/> to a <see cref="SKPath"/>.
/// </summary>
internal sealed class PolylineToPdf : IConverter<Polyline, SKPath>
{
    /// <inheritdoc cref="PolylineToPdf" />
    public SKPath Convert(Polyline polyline)
    {
        Vector3ToPdf converter = new();
        SKPath path = new();
        path.AddPoly(polyline.Vertices.Select(converter.Convert).ToArray());
        return path;
    }
}
