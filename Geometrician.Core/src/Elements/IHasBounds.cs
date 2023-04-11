namespace Geometrician.Core.Elements;

/// <summary>
/// The bounds of an <see cref="Element"/>
/// </summary>
public interface IHasBounds
{
    /// <inheritdoc cref="IHasBounds"/>
    public BBox3 Bounds { get; }
}
