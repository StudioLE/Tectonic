using System.IO;
using Elements.Serialization.IFC;
using Lineweights.Core.Documents;

namespace Lineweights.IFC;

/// <summary>
/// Methods to add PDF to <see cref="IAssetBuilder"/>.
/// </summary>
public static class AssetBuilderExtensions
{
    /// <inheritdoc cref="Asset"/>
    public static IAssetBuilder ConvertModelToIfc(this IAssetBuilder @this, DocumentInformation? doc = null)
    {
        @this.AddStep(async (model, storageStrategy) =>
        {
            doc ??= new()
            {
                Name = "IFC of Model"
            };

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
                Info = doc,
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

            string fileName = doc.Id + ".ifc";
            return await storageStrategy.WriteAsync(asset, fileName, stream);
        });
        return @this;
    }
}
