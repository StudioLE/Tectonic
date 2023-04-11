namespace Geometrician.PDF.From.Geometry;

/// <summary>
/// Convert a <see cref="Color"/> to a <see cref="SKColor"/>.
/// </summary>
internal sealed class ColorToPdf : IConverter<Color, SKColor>
{
    /// <inheritdoc cref="ColorToPdf"/>
    public SKColor Convert(Color color)
    {
        byte r = UParameterToByte(color.Red);
        byte g = UParameterToByte(color.Green);
        byte b = UParameterToByte(color.Blue);
        byte a = UParameterToByte(color.Alpha);
        return new(r, g, b, a);
    }

    private static byte UParameterToByte(double u)
    {
        return (byte)(u * 255);
    }
}
