namespace Lineweights.SVG;

/// <summary>
/// An SVG Document as an <see cref="XDocument"/>.
/// </summary>
public sealed class SvgDocument : XDocument
{
    /// <inheritdoc cref="SvgDocument"/>
    public SvgDocument(params object[] content) : base(new("1.0", "utf-8", "no"), content)
    {
    }
}
