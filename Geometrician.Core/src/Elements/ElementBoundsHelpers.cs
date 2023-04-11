using StudioLE.Core.Results;

namespace Geometrician.Core.Elements;

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
    public static IResult<BBox3> TryGetBounds(this Element @this)
    {
        BBox3 bounds = @this switch
        {
            GeometricElement geometric => geometric.Bounds(),
            ElementInstance instance => instance.Bounds(),
            _ => new(@this)
        };
        if (bounds.IsInfinite())
            return IResult<BBox3>.Error("Bounds were infinite.");
        if (bounds.IsInverted())
            return IResult<BBox3>.Error("Bounds were inverted.");
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
    public static IResult<BBox3> TryGetTransformedBounds(this Element @this)
    {
        BBox3 bounds = @this switch
        {
            GeometricElement geometric => geometric.TransformedBounds(),
            ElementInstance instance => instance.TransformedBounds(),
            _ => new(@this)
        };
        if (bounds.IsInfinite())
            return new Failure<BBox3>("Bounds were infinite.");
        if (bounds.IsInverted())
            return new Failure<BBox3>("Bounds were inverted.");
        return new Success<BBox3>(bounds);
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

    /// <summary>
    /// Add a representation of the bounds of the element to the model.
    /// The <see cref="BBox3"/> of the element is determined by <see cref="ElementBoundsHelpers.TryGetTransformedBounds"/>
    /// and then converted to <see cref="ModelCurve"/>.
    /// </summary>
    public static void AddBounds(this Model @this, Element element, Material? material = null)
    {
        material ??= MaterialByName("White");
        IResult<BBox3> result = element.TryGetTransformedBounds();
        if (result is Success<BBox3> success)
            @this.AddElements(success.Value.ToModelCurves(null, material));
    }

    /// <summary>
    /// Add a representation of the bounds of each individual element to the model.
    /// The <see cref="BBox3"/> of the element is determined by <see cref="ElementBoundsHelpers.TryGetTransformedBounds"/>
    /// and then converted to <see cref="ModelCurve"/>.
    /// </summary>
    public static void AddBounds(this Model @this, IEnumerable<Element> elements)
    {
        foreach (Element element in elements)
            @this.AddBounds(element);
    }
}
