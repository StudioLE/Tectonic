using System.Diagnostics;
using System.IO;

namespace Lineweights.Workflows.Results;

/// <summary>
/// A <see cref="IResultStrategy"/> to open results.
/// </summary>
public sealed class OpenAsFile : IResultStrategy
{
    /// <summary>
    /// Should the file be opened.
    /// </summary>
    public bool IsOpenEnabled { get; set; } = true;

    /// <summary>
    /// The path of the created file.
    /// </summary>
    public Func<IStorageStrategy, Model, DocumentInformation, Result> Builder { get; set; } = ResultBuilder.Default;

    /// <inheritdoc cref="OpenAsFile"/>
    public OpenAsFile(params string[] fileExtensions)
    {
        if (!fileExtensions.Any())
            return;
        Builder = (storageStrategy, model, metadata) =>
        {
            ResultBuilder builder = new ResultBuilder(storageStrategy)
                .Metadata(metadata);
            if (fileExtensions.Contains(".glb"))
                builder = builder.AddModelConvertedToGlb(model);
            if (fileExtensions.Contains(".ifc"))
                builder = builder.AddModelConvertedToIfc(model);
            if (fileExtensions.Contains(".json"))
                builder = builder.AddModelConvertedToJson(model);
            if (fileExtensions.Contains(".svg"))
                builder = builder.AddCanvasesConvertedToSvg(model);
            return builder.Build();
        };
    }

    /// <inheritdoc cref="OpenAsFile"/>
    public Result Execute(Model model, DocumentInformation metadata)
    {
        Result result = Builder(new FileStorageStrategy(), model, metadata);
        if (IsOpenEnabled)
            RecursiveOpen(result);
        return result;
    }

    private static void RecursiveOpen(Result result)
    {
        if (File.Exists(result.Metadata.Location?.AbsolutePath))
            Process.Start(new ProcessStartInfo(result.Metadata.Location!.AbsolutePath)
            {
                UseShellExecute = true
            });
        foreach (Result child in result.Children)
            RecursiveOpen(child);
    }
}
