using StudioLE.Core.System;

namespace Lineweights.Workflows.Verification;

/// <summary>
/// Methods to help verify test results using <see cref="VerifierBase{T}"/>.
/// Engine specific logic is abstracted to <see cref="IVerifyContext"/>.
/// </summary>
/// <remarks>
/// </remarks>
public static class Verify
{
    private static Type? _contextType;

    public static bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Verify <paramref name="elements"/> by their <see cref="BBox3"/>.
    /// </summary>
    public static void ElementsByBounds(IEnumerable<Element> elements)
    {
        // TODO: Use .Bounds() or .TransformedBounds()?
        BBox3[] bounds = elements
            .Select(x => new BBox3(x))
            .ToArray();
        AsJson(bounds);
    }

    /// <summary>
    /// Verify <paramref name="elements"/> by their <see cref="Transform"/>.
    /// </summary>
    public static void ElementsByTransform(IEnumerable<GeometricElement> elements)
    {
        Transform[] transforms = elements
            .Select(x => x.Transform)
            .ToArray();
        AsJson(transforms);
    }


    /// <summary>
    /// Verify <paramref name="modelCurves"/> by their <see cref="ModelCurve.Curve"/>.
    /// </summary>
    public static void ModelCurvesByCurve(IEnumerable<ModelCurve> modelCurves)
    {
        Curve[] curves = modelCurves
            .Select(x => x.Curve.Transformed(x.Transform))
            .ToArray();
        AsJson(curves);
    }

    /// <summary>
    /// Verify any <see cref="Elements"/> geometry by its serialised form.
    /// </summary>
    public static void Geometry(params object[] geometry)
    {
        AsJson(geometry);
    }

    /// <summary>
    /// Verify <paramref name="actual"/> as JSON.
    /// </summary>
    private static void AsJson(object actual)
    {
        if (!IsEnabled)
            return;
        IVerifyContext context = GetContext();
        JsonVerifier verifier = new(context);
        _ = verifier.Execute(actual);
    }

    /// <summary>
    /// Verify a string.
    /// </summary>
    public static void String(string @string, string fileExtension)
    {
        if (!IsEnabled)
            return;
        IVerifyContext context = GetContext();
        StringVerifier verifier = new(context, fileExtension);
        _ = verifier.Execute(@string);
    }

    private static IVerifyContext GetContext()
    {
        if (_contextType is null)
        {
            AssemblyHelpers.LoadAllAssemblies("Lineweights.Workflows.");
            _contextType = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => typeof(IVerifyContext).IsAssignableFrom(x)
                                     && x.IsClass
                                     && !x.IsAbstract);
            if (_contextType is null)
                throw new("Failed to Verify. Could not determine the context.");
        }
        return (IVerifyContext)Activator.CreateInstance(_contextType);

    }
}
