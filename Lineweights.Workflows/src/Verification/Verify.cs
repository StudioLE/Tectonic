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
    public static async Task ElementsByBounds(IEnumerable<Element> elements)
    {
        // TODO: Use .Bounds() or .TransformedBounds()?
        BBox3[] bounds = elements
            .Select(x => new BBox3(x))
            .ToArray();
        await AsJson(bounds);
    }

    /// <summary>
    /// Verify <paramref name="elements"/> by their <see cref="Transform"/>.
    /// </summary>
    public static async Task ElementsByTransform(IEnumerable<GeometricElement> elements)
    {
        Transform[] transforms = elements
            .Select(x => x.Transform)
            .ToArray();
        await AsJson(transforms);
    }


    /// <summary>
    /// Verify <paramref name="modelCurves"/> by their <see cref="ModelCurve.Curve"/>.
    /// </summary>
    public static async Task ModelCurvesByCurve(IEnumerable<ModelCurve> modelCurves)
    {
        Curve[] curves = modelCurves
            .Select(x => x.Curve.Transformed(x.Transform))
            .ToArray();
        await AsJson(curves);
    }

    /// <summary>
    /// Verify any <see cref="Elements"/> geometry by its serialised form.
    /// </summary>
    public static async Task Geometry(params object[] geometry)
    {
        await AsJson(geometry);
    }

    /// <summary>
    /// Verify <paramref name="actual"/> as JSON.
    /// </summary>
    private static async Task AsJson(object actual)
    {
        if (!IsEnabled)
            return;
        IVerifyContext context = GetContext();
        JsonVerifier verifier = new(context);
        await verifier.Execute(actual);
    }

    /// <summary>
    /// Verify <paramref name="actual"/> as JSON.
    /// </summary>
    public static async Task AsJson(object expected, object actual)
    {
        if (!IsEnabled)
            return;
        IVerifyContext context = GetContext();
        JsonVerifier verifier = new(context);
        await verifier.Execute(expected, actual);
    }

    /// <summary>
    /// Verify a string.
    /// </summary>
    public static async Task String(string @string)
    {
        if (!IsEnabled)
            return;
        IVerifyContext context = GetContext();
        StringVerifier verifier = new(context, ".txt");
        await verifier.Execute(@string);
    }

    /// <summary>
    /// Verify a string.
    /// </summary>
    public static async Task String(string expected, string actual)
    {
        if (!IsEnabled)
            return;
        IVerifyContext context = GetContext();
        StringVerifier verifier = new(context, ".txt");
        await verifier.Execute(expected, actual);
    }

    /// <summary>
    /// Verify a file.
    /// </summary>
    public static async Task File(FileInfo file)
    {
        if (!IsEnabled)
            return;
        IVerifyContext context = GetContext();
        FileVerifier verifier = new(context, file.Extension);
        await verifier.Execute(file);
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
