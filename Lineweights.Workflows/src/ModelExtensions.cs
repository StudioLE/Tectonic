using Ardalis.Result;

namespace Lineweights.Workflows;

/// <summary>
/// Methods to help with <see cref="Model"/>.
/// </summary>
public static class ModelExtensions
{
    /// <summary>
    /// Add a representation of the bounds of the element to the model.
    /// The <see cref="BBox3"/> of the element is determined by <see cref="ElementBoundsHelpers.TryGetTransformedBounds"/>
    /// and then converted to <see cref="ModelCurve"/>.
    /// </summary>
    public static void AddBounds(this Model @this, Element element, Material? material = null)
    {
        material ??= MaterialByName("White");
        Result<BBox3> result = element.TryGetTransformedBounds();
        if (result.IsSuccess)
            @this.AddElements(result.Value.ToModelCurves(null, material));
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
