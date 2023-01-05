using System.IO;
using Elements.Serialization.IFC;
using Lineweights.Core.Documents;
using StudioLE.Core.System;

namespace Lineweights.IFC;

public class IfcAssetFactory : IAssetFactory
{
    private readonly DocumentInformation _info;

    public IfcAssetFactory(DocumentInformation? info = null)
    {
        _info = info ?? new()
        {
            Name = "IFC of Model"
        };
    }

    /// <inheritdoc/>
    public IEnumerable<Task<Asset>> Execute(IAssetBuilderContext context)
    {
        string tempPath = Path.GetTempFileName();

        // Temporarily capture the console output from ToIFC
        string console;
        using (StringWriter consoleWriter = new())
        {
            Console.SetOut(consoleWriter);
            context.Model.ToIFC(tempPath);
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
            Info = _info.CloneAs(),
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

        Task<Asset> task = context.StorageStrategy.WriteAsync(asset, fileName, stream);
        return new [] { task };
    }
}
