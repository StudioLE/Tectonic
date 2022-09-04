using System.IO;
using System.Text;
using Lineweights.Workflows.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
    public static readonly Encoding Encoding = Encoding.UTF8;

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

    /// <summary>
    /// Verify a file.
    /// </summary>
    public static void File(FileInfo file)
    {
        if (!IsEnabled)
            return;
        IVerifyContext context = GetContext();
        FileVerifier verifier = new(context, file.Extension);
        _ = verifier.Execute(file);
    }

    internal static IVerifyContext GetContext()
    {
        IServiceProvider services = Services.GetInstance();
        IVerifyContext? context = services.GetService<IVerifyContext>();
        if (context is not null)
            return context;
        RegisterContexts();
        Services.Reset();
        services = Services.GetInstance();
        return services.GetRequiredService<IVerifyContext>();
    }

    private static void RegisterContexts()
    {
        AssemblyHelpers.LoadAllAssemblies("Lineweights.Workflows.");
        IEnumerable<Type> contextTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(IVerifyContext).IsAssignableFrom(x)
                                 && x.IsClass
                                 && !x.IsAbstract);

        Services.ConfigureServices = (_, services) =>
        {
            foreach (Type type in contextTypes)
                services.AddTransient(typeof(IVerifyContext), type);
        };
    }
}
