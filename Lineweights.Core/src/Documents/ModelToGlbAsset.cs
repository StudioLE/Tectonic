using System.IO;
using Elements.Serialization.glTF;
using StudioLE.Core.Conversion;

namespace Lineweights.Core.Documents;

/// <summary>
/// Convert a <see cref="Model"/> to a GLB file
/// referenced as <see cref="Asset"/>.
/// </summary>
public class ModelToGlbAsset : IConverter<Model, Task<Asset>>
{
    private readonly IStorageStrategy _storageStrategy;

    /// <inheritdoc cref="ModelToGlbAsset"/>
    public ModelToGlbAsset(IStorageStrategy storageStrategy)
    {
        _storageStrategy = storageStrategy;
    }

    /// <inheritdoc />
    public Task<Asset> Convert(Model model)
    {
        Asset asset = new()
        {
            Info = new()
            {
                Name = "GlTF of Model"
            },
            ContentType = "model/gltf-binary"
        };

        string fileName = asset.Info.Id + ".glb";
        string tempPath = Path.GetTempFileName();
        model.ToGlTF(tempPath, out List<BaseError> errors);

        if (errors.Any())
            asset.Errors = errors
                .Select(x => x.Message)
                .Prepend("Failed to convert Model to GLB.")
                .ToArray();

        if (!File.Exists(tempPath))
            throw new FileNotFoundException("Failed to write GLB. Temp file does not exist.");
        Stream stream = File.OpenRead(tempPath);

        Task<Asset> task = _storageStrategy.WriteAsync(asset, fileName, stream);
        return task;
    }
}
