using System.IO;
using Elements.Serialization.glTF;
using Lineweights.Core.Documents;

namespace Lineweights.Workflows.Documents;

public static class AssetBuilderExtensions
{
    /// <inheritdoc cref="Asset"/>
    public static T AddAssets<T>(this T @this, params Asset[] assets) where T : IAssetBuilder
    {
        IAssetBuilder.BuildTask build = (_, _) =>
            assets.Select(Task.FromResult);
        @this.Tasks.Add(build);
        return @this;
    }

    /// <inheritdoc cref="Asset"/>
    public static T ConvertModelToGlb<T>(this T @this, DocumentInformation? doc = null) where T : IAssetBuilder
    {
        IAssetBuilder.BuildTask build = (model, storageStrategy) =>
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


            Task<Asset> task = storageStrategy.WriteAsync(asset, fileName, stream);
            return new [] { task };
        };
        @this.Tasks.Add(build);
        return @this;
    }

    /// <inheritdoc cref="Asset"/>
    public static T ConvertModelToJson<T>(this T @this, DocumentInformation? doc = null) where T : IAssetBuilder
    {
        IAssetBuilder.BuildTask build = (model, _) =>
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
            return new [] { Task.FromResult(asset) };
        };
        @this.Tasks.Add(build);
        return @this;
    }
}
