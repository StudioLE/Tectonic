namespace Geometrician.SVG;

/// <summary>
/// An SVG element as an <see cref="XElement"/> with the SVG namespace.
/// </summary>
public sealed class SvgElement : XElement
{
    public static readonly XNamespace Namespace = "http://www.w3.org/2000/svg";

    /// <inheritdoc cref="SvgElement"/>
    public SvgElement(XElement other) : base(other)
    {
    }

    /// <inheritdoc cref="SvgElement"/>
    public SvgElement(string name) : base(Namespace + name)
    {
    }

    /// <inheritdoc cref="SvgElement"/>
    public SvgElement(string name, object content) : base(Namespace + name, content)
    {
    }

    /// <inheritdoc cref="SvgElement"/>
    public SvgElement(string name, params object[] content) : base(Namespace + name, content)
    {
    }
}
