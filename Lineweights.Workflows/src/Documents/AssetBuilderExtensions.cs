using System.IO;
using Elements.Serialization.glTF;
using Lineweights.Core.Documents;

namespace Lineweights.Workflows.Documents;

public static class AssetBuilderExtensions
{
    /// <inheritdoc cref="Asset"/>
    public static IAssetBuilder AddAssets(this IAssetBuilder @this, params Asset[] assets)
    {
        foreach (Asset asset in assets)
            @this.AddAsset(asset);
        return @this;
    }

    /// <inheritdoc cref="Asset"/>
    public static IAssetBuilder ConvertModelToGlb(this IAssetBuilder @this, DocumentInformation? doc = null)
    {
        @this.AddStep(async (model, storageStrategy) =>
        {
            doc ??= new()
            {
                Name = "GlTF of Model"
            };

            string fileName = doc.Id + ".glb";
            string tempPath = Path.GetTempFileName();
            model.ToGlTF(tempPath, out List<BaseError> errors);

            Asset asset = new()
            {
                Info = doc,
                ContentType = "model/gltf-binary"
            };
            if (errors.Any())
                asset.Errors = errors
                    .Select(x => x.Message)
                    .Prepend("Failed to convert Model to GLB.")
                    .ToArray();

            if (!File.Exists(tempPath))
                throw new FileNotFoundException("Failed to write GLB. Temp file does not exist.");
            Stream stream = File.OpenRead(tempPath);

            return await storageStrategy.WriteAsync(asset, fileName, stream);
        });
        return @this;
    }

    /// <inheritdoc cref="Asset"/>
    public static IAssetBuilder ConvertModelToJson(this IAssetBuilder @this, DocumentInformation? doc = null)
    {
        @this.AddStep((model, _) =>
        {
            doc ??= new()
            {
                Name = "JSON of Model"
            };

            string json = model.ToJson();

            Asset asset = new()
            {
                Info = doc,
                ContentType = "application/json",
                Content = json
            };
            return Task.FromResult(asset);
        });

        return @this;
    }
}
