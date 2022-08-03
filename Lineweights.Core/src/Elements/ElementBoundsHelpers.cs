using Ardalis.Result;

namespace Lineweights.Core.Elements;

/// <summary>
/// Methods to get the <see cref="BBox3"/> of <see cref="Element"/>.
/// </summary>
public static class ElementBoundsHelpers
{
#if false
    /// <summary>
    /// Try and get the bounds of the <see cref="Element"/>.
    /// Returns false if the bounds could not be determined.
    /// </summary>
    [Obsolete("Produces inconsistent results.")]
    public static Result<BBox3> TryGetBounds(this Element @this)
    {
        BBox3 bounds = @this switch
        {
            GeometricElement geometric => geometric.Bounds(),
            ElementInstance instance => instance.Bounds(),
            _ => new(@this)
        };
        if (bounds.IsInfinite())
            return Result<BBox3>.Error("Bounds were infinite.");
        if (bounds.IsInverted())
            return Result<BBox3>.Error("Bounds were inverted.");
        return bounds;
    }
#endif


#if false

    /// <summary>
    /// Get the bounds of an <see cref="ElementInstance"/>.
    /// </summary>
    [Obsolete("Produces inconsistent results.")]
    public static BBox3 Bounds(this ElementInstance @this)
    {
        @this.BaseDefinition.UpdateRepresentations();
        return new(@this);
    }
    /// <summary>
    /// Get the bounds of an <see cref="GeometricElement"/>
    /// transformed by <see cref="GeometricElement.Transform"/>.
    /// </summary>
    [Obsolete("Produces inconsistent results.")]
    public static BBox3 TransformedBounds(this GeometricElement @this)
    {
        return @this
            .Bounds()
            .Transformed(@this.Transform);
    }


    /// <summary>
    /// Get the bounds of an <see cref="GeometricElement"/>
    /// transformed by the base <see cref="GeometricElement.Transform"/>
    /// and the <see cref="ElementInstance.Transform"/>.
    /// </summary>
    public static BBox3 TransformedBounds(this ElementInstance @this)
    {
        return @this
            .BaseDefinition
            .TransformedBounds()
            .Transformed(@this.Transform);
    }
#endif

    /// <summary>
    /// Try and get the bounds of the <see cref="Element"/>.
    /// Returns false if the bounds could not be determined.
    /// </summary>
    public static Result<BBox3> TryGetTransformedBounds(this Element @this)
    {
        BBox3 bounds = @this switch
        {
            GeometricElement geometric => geometric.TransformedBounds(),
            ElementInstance instance => instance.TransformedBounds(),
            _ => new(@this)
        };
        if (bounds.IsInfinite())
            return Result<BBox3>.Error("Bounds were infinite.");
        if (bounds.IsInverted())
            return Result<BBox3>.Error("Bounds were inverted.");
        return bounds;
    }

    /// <summary>
    /// Get the bounds of a <see cref="GeometricElement"/>
    /// transformed by <see cref="GeometricElement.Transform"/>.
    /// </summary>
    public static BBox3 TransformedBounds(this GeometricElement @this)
    {
        if (@this is IHasBounds hasBounds)
            return hasBounds
                .Bounds
                .Transformed(@this.Transform);
        @this.UpdateRepresentations();
        return new(@this);
    }

    /// <summary>
    /// Get the bounds of an <see cref="GeometricElement"/>
    /// transformed by the base <see cref="GeometricElement.Transform"/>
    /// and the <see cref="ElementInstance.Transform"/>.
    /// </summary>
    public static BBox3 TransformedBounds(this ElementInstance @this)
    {
        if (@this is IHasBounds)
            throw new($"Failed to get transformed bounds. {nameof(ElementInstance)} should not implement IHasBounds.");
        return @this
            .BaseDefinition
            .TransformedBounds()
            .Transformed(@this.Transform);
    }

    private static BBox3 Transformed(this BBox3 bounds, Transform transform)
    {
        return new(transform.OfPoint(bounds.Min), transform.OfPoint(bounds.Max));
    }

    /// <inheritdoc cref="GeometricElement.CreateInstance(Transform, string)"/>
    public static ElementInstance CreateInstance(this GeometricElement @this, string? name = null)
    {
        return @this.CreateInstance(new(), name ?? @this.Name);
    }
}
