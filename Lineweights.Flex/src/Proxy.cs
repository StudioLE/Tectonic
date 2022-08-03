using Ardalis.Result;
using Lineweights.Flex.Coordination;

namespace Lineweights.Flex;

/// <summary>
/// A <see href="https://refactoring.guru/design-patterns/proxy">proxy</see> which represents an
/// <see cref="ElementInstance"/> of <see cref="BaseDefinition"/>.
/// </summary>
internal sealed class Proxy
{
    /// <inheritdoc cref="ElementInstance.BaseDefinition"/>
    public GeometricElement BaseDefinition { get; }

    /// <summary>
    /// The bounds of the proxied element.
    /// </summary>
    public BBox3 Bounds { get; }

    /// <summary>
    /// The minimum spacing around the referenced <see cref="GeometricElement"/>.
    /// </summary>
    public Spacing MinSpacing { get; }

    /// <summary>
    /// The vector from the min point to the origin point.
    /// </summary>
    public Vector3 Offset { get; }

    /// <summary>
    /// The vector from the base point to the min point.
    /// </summary>
    public Vector3 Alignment { get; set; }

    /// <summary>
    /// The vector from the cap line to the next base line.
    /// </summary>
    public Vector3 Spacing { get; set; }

    /// <summary>
    /// The vector from the start point to the base point.
    /// </summary>
    public Vector3 Coordination { get; set; }

    /// <summary>
    /// The vector from the container center point to the start point.
    /// </summary>
    public Vector3 SettingOut { get; set; }

    /// <summary>
    /// The translation from the container center point to the origin of the element.
    /// </summary>
    public Vector3 Translation => SettingOut
                                  + Coordination
                                  + Spacing
                                  + Alignment
                                  + Offset;

    /// <inheritdoc cref="Proxy"/>
    public Proxy(Proxy proxy)
    {
        BaseDefinition = proxy.BaseDefinition;
        Bounds = proxy.Bounds;
        MinSpacing = proxy.MinSpacing;
        Offset = proxy.Offset;
        Alignment = proxy.Alignment;
        Spacing = proxy.Spacing;
        Coordination = proxy.Coordination;
        SettingOut = proxy.SettingOut;
    }

    /// <inheritdoc cref="Proxy"/>
    public Proxy(GeometricElement element)
    {
        BaseDefinition = element;
        // TODO: Replace GetBounds with element.TransformedBounds()
        Bounds = GetBounds(element);
        if (Bounds.IsInverted())
            throw new($"Failed to construct {nameof(Proxy)}. The element bounding box was inverted.");
        Result<Spacing> spacing = element.GetProperty<Spacing>();
        if (spacing.IsSuccess)
            MinSpacing = spacing;
    }

    /// <inheritdoc cref="Proxy"/>
    public Proxy(ElementInstance instance)
    {
        BaseDefinition = instance.BaseDefinition;
        Bounds = instance.TransformedBounds();
        if (Bounds.IsInverted())
            throw new($"Failed to construct {nameof(Proxy)}. The element bounding box was inverted.");
        Result<Spacing> spacing = instance.BaseDefinition.GetProperty<Spacing>();
        if (spacing.IsSuccess)
            MinSpacing = spacing;
    }


    /// <summary>
    /// Get the bounds of a <see cref="GeometricElement"/>.
    /// </summary>
    public static BBox3 GetBounds(GeometricElement @this)
    {
        @this.UpdateRepresentations();
        if (@this is IHasBounds hasBounds)
            return hasBounds.Bounds;
        return new(@this);
    }
}
