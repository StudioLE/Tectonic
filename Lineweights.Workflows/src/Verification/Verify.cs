using Ardalis.Result;
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

    /// <summary>
    /// Verify <paramref name="elements"/> by their <see cref="BBox3"/>.
    /// </summary>
    public static bool ElementsByBounds(IEnumerable<Element> elements)
    {
        // TODO: Use .Bounds() or .TransformedBounds()?
        BBox3[] bounds = elements
            .Select(x => new BBox3(x))
            .ToArray();
        return AsJson(bounds);
    }

    /// <summary>
    /// Verify <paramref name="elements"/> by their <see cref="Transform"/>.
    /// </summary>
    public static bool ElementsByTransform(IEnumerable<GeometricElement> elements)
    {
        Transform[] transforms = elements
            .Select(x => x.Transform)
            .ToArray();
        return AsJson(transforms);
    }


    /// <summary>
    /// Verify <paramref name="modelCurves"/> by their <see cref="ModelCurve.Curve"/>.
    /// </summary>
    public static bool ModelCurvesByCurve(IEnumerable<ModelCurve> modelCurves)
    {
        Curve[] curves = modelCurves
            .Select(x => x.Curve.Transformed(x.Transform))
            .ToArray();
        return AsJson(curves);
    }

    /// <summary>
    /// Verify any <see cref="Elements"/> geometry by its serialised form.
    /// </summary>
    public static bool Geometry(params object[] geometry)
    {
        return AsJson(geometry);
    }

    /// <summary>
    /// Verify <paramref name="actual"/> as JSON.
    /// </summary>
    private static bool AsJson(object actual)
    {
        IVerifyContext context = GetContext();
        JsonVerifier verifier = new(context);
        Result<bool> result = verifier.Execute(actual);
        return result.IsSuccess;
    }

    /// <summary>
    /// Verify a string.
    /// </summary>
    public static bool String(string @string, string fileExtension)
    {
        IVerifyContext context = GetContext();
        StringVerifier verifier = new(context, fileExtension);
        Result<bool> result = verifier.Execute(@string);
        return result.IsSuccess;
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
