using Ardalis.Result;
using StudioLE.Core.System;

namespace Lineweights.Results.Constraints;

/// <summary>
/// Methods to help verify test results using <see cref="Verifier"/>.
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
        return Execute(bounds);
    }

    /// <summary>
    /// Verify <paramref name="elements"/> by their <see cref="Transform"/>.
    /// </summary>
    public static bool ElementsByTransform(IEnumerable<GeometricElement> elements)
    {
        Transform[] transforms = elements
            .Select(x => x.Transform)
            .ToArray();
        return Execute(transforms);
    }


    /// <summary>
    /// Verify <paramref name="modelCurves"/> by their <see cref="ModelCurve.Curve"/>.
    /// </summary>
    public static bool ModelCurvesByCurve(IEnumerable<ModelCurve> modelCurves)
    {
        Curve[] curves = modelCurves
            .Select(x => x.Curve.Transformed(x.Transform))
            .ToArray();
        return Execute(curves);
    }

    /// <summary>
    /// Verify any <see cref="Elements"/> geometry by its serialised form.
    /// </summary>
    public static bool Geometry(params object[] geometry)
    {
        return Execute(geometry);
    }

    /// <summary>
    /// Verify a string.
    /// </summary>
    public static bool String(string @string)
    {
        return Execute(@string);
    }

    /// <summary>
    /// Verify <paramref name="actual"/>.
    /// </summary>
    private static bool Execute(object actual)
    {
        IVerifyContext context = GetContext();
        Verifier verifier = new(context);
        Result<bool> result = verifier.Execute(actual);
        return result.IsSuccess;
    }

    private static IVerifyContext GetContext()
    {
        if (_contextType is null)
        {
            AssemblyHelpers.LoadAllAssemblies("Lineweights.Results.");
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
