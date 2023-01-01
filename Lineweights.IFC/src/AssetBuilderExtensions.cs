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
    public static T ConvertModelToIfc<T>(this T @this, DocumentInformation? doc = null) where T : IAssetBuilder
    {
        IAssetBuilder.BuildTask build = (model, storageStrategy) =>
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

            Task<Asset> task = storageStrategy.WriteAsync(asset, fileName, stream);
            return new [] { task };
        };
        @this.Tasks.Add(build);
        return @this;
    }
}
