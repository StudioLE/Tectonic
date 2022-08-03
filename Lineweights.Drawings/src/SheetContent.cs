namespace Lineweights.Drawings;

/// <summary>
/// The content space or drawing space of a <see cref="Sheet"/> which contains the <see cref="Views"/>
/// The size and position are determined by the space leftover after the <see cref="Sheet.Title"/> is placed.
/// </summary>
public sealed class SheetContent : Canvas
{
    /// <summary>
    /// The views positioned within the <see cref="SheetContent"/>.
    /// </summary>
    public IReadOnlyCollection<ElementInstance> Views { get; set; } = Array.Empty<ElementInstance>();

    /// <inheritdoc cref="SheetContent"/>
    internal SheetContent()
    {
    }
}
