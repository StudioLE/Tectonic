using StudioLE.Core.System;
using StudioLE.Verify;

namespace Lineweights.Diagnostics;

/// <summary>
/// Methods to help verify test results using <see cref="VerifierBase{T}"/>.
/// Engine specific logic is abstracted to <see cref="IVerifyContext"/>.
/// </summary>
/// <remarks>
/// </remarks>
public static class VerifyExtensions
{
    /// <summary>
    /// Verify <paramref name="elements"/> by their <see cref="BBox3"/>.
    /// </summary>
    public static async Task ElementsByBounds(this Verify @this, IEnumerable<Element> elements)
    {
        // TODO: Use .Bounds() or .TransformedBounds()?
        BBox3[] bounds = elements
            .Select(x => new BBox3(x))
            .ToArray();
        await @this.AsJson(bounds);
    }

    /// <summary>
    /// Verify <paramref name="elements"/> by their <see cref="Transform"/>.
    /// </summary>
    public static async Task ElementsByTransform(this Verify @this, IEnumerable<GeometricElement> elements)
    {
        Transform[] transforms = elements
            .Select(x => x.Transform)
            .ToArray();
        await @this.AsJson(transforms);
    }


    /// <summary>
    /// Verify <paramref name="modelCurves"/> by their <see cref="ModelCurve.Curve"/>.
    /// </summary>
    public static async Task ModelCurvesByCurve(this Verify @this, IEnumerable<ModelCurve> modelCurves)
    {
        Curve[] curves = modelCurves
            .Select(x => x.Curve.Transformed(x.Transform))
            .ToArray();
        await @this.AsJson(curves);
    }

    /// <summary>
    /// Verify any <see cref="Elements"/> geometry by its serialised form.
    /// </summary>
    public static async Task Geometry(this Verify @this, params object[] geometry)
    {
        await @this.AsJson(geometry);
    }

    /// <summary>
    /// Verify a <see cref="Model"/> by the element ids.
    /// </summary>
    public static async Task ByElementIds(this Verify @this, Model expected, Model actual)
    {
        IReadOnlyCollection<string> expectedElements = GetElementIds(expected);
        IReadOnlyCollection<string> actualElements = GetElementIds(actual);
        await @this.String(expectedElements.Join(), actualElements.Join());
    }

    private static IReadOnlyCollection<string> GetElementIds(Model model)
    {
        return model
            .Elements
            .Values
            .Select(element => $"{element.Id} {element.GetType()}")
            .OrderBy(x => x)
            .ToArray();
    }
}
