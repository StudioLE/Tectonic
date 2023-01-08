using System.IO;
using Elements.Serialization.IFC;
using Lineweights.Core.Documents;

namespace Lineweights.IFC;

/// <summary>
/// Convert a <see cref="Model"/> to an IFC file
/// referenced as <see cref="Asset"/>.
/// </summary>
public class ModelToIfcAsset : IConverter<Model, Task<Asset>>
{
    private readonly IStorageStrategy _storageStrategy;


    /// <inheritdoc cref="ModelToIfcAsset"/>
    public ModelToIfcAsset(IStorageStrategy storageStrategy)
    {
        _storageStrategy = storageStrategy;
    }

    /// <inheritdoc />
    public async Task<Asset> Convert(Model model)
    {
        string tempPath = Path.GetTempFileName();

        // Temporarily capture the console output from ToIFC
        string console;
        using (StringWriter consoleWriter = new())
        {
            Console.SetOut(consoleWriter);
            model.ToIFC(tempPath);
            console = consoleWriter.ToString();
        }

        // Reset the console output
        StreamWriter standardOutputWriter = new(Console.OpenStandardOutput())
        {
            AutoFlush = true
        };
        Console.SetOut(standardOutputWriter);

        Asset asset = new()
        {
            Info = new()
            {
                Name = "IFC of Model"
            },
            ContentType = "application/x-step"
        };
        if (!string.IsNullOrWhiteSpace(console))
            asset.Errors = new[]
            {
                "Failed to convert Model to IFC.",
                console
            };

        if (!File.Exists(tempPath))
            throw new FileNotFoundException("Failed to write GLB. Temp file does not exist.");
        Stream stream = File.OpenRead(tempPath);

        string fileName = asset.Info.Id + ".ifc";

        Task<Asset> task = _storageStrategy.WriteAsync(asset, fileName, stream);
        return await task;
    }
}
