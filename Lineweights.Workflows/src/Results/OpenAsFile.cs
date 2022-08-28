using System.Diagnostics;
using System.IO;
using System.Text;
using Lineweights.Workflows.Containers;
using StudioLE.Core.System.IO;

namespace Lineweights.Workflows.Results;

/// <summary>
/// A <see cref="IResultStrategy"/> to open results.
/// </summary>
public sealed class OpenAsFile : IResultStrategy
{
    private readonly IStorageStrategy _storageStrategy = new FileStorageStrategy();

    /// <summary>
    /// Should the file be opened.
    /// </summary>
    public bool IsOpenEnabled { get; set; } = true;

    /// <summary>
    /// The path of the created file.
    /// </summary>
    public Func<IStorageStrategy, Model, DocumentInformation, ContainerBuilder> Builder { get; set; } = ContainerBuilder.Default;

    /// <inheritdoc cref="OpenAsFile"/>
    public OpenAsFile(params string[] fileExtensions)
    {
        if (!fileExtensions.Any())
            return;
        Builder = (storageStrategy, model, doc) =>
        {
            ContainerBuilder builder = new(storageStrategy, doc);
            if (fileExtensions.Contains(".glb"))
                builder = builder.ConvertModelToGlb(model);
            if (fileExtensions.Contains(".ifc"))
                builder = builder.ConvertModelToIfc(model);
            if (fileExtensions.Contains(".json"))
                builder = builder.ConvertModelToJson(model);
            if (fileExtensions.Contains(".svg"))
                builder = builder.ExtractViewsAndConvertToSvg(model);
            return builder;
        };
    }

    /// <inheritdoc cref="OpenAsFile"/>
    public async Task<Container> Execute(Model model, DocumentInformation doc)
    {
        ContainerBuilder builder = Builder(_storageStrategy, model, doc);
        Container container = await builder.Build();
        await RecursiveWriteContent(container);
        if (IsOpenEnabled)
            RecursiveOpen(container);
        return container;
    }

    private async Task RecursiveWriteContent(Container container)
    {
        if (container.Content is not null)
        {
            string fileName = container.Info.Id + (container.ContentType.GetExtensionByContentType() ?? ".txt");
            byte[] byteArray = Encoding.ASCII.GetBytes(container.Content);
            MemoryStream stream = new(byteArray);
            _ = await _storageStrategy.WriteAsync(container, fileName, stream);
        }
        foreach (Container child in container.Children)
            await RecursiveWriteContent(child);
    }

    private static void RecursiveOpen(Container container)
    {
        if (File.Exists(container.Info.Location?.AbsolutePath))
            Process.Start(new ProcessStartInfo(container.Info.Location!.AbsolutePath)
            {
                UseShellExecute = true
            });
        foreach (Container child in container.Children)
            RecursiveOpen(child);
    }
}
