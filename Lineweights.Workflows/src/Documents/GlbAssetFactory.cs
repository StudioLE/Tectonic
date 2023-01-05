using System.IO;
using Elements.Serialization.glTF;
using Lineweights.Core.Documents;
using StudioLE.Core.System;

namespace Lineweights.Workflows.Documents;

public class GlbAssetFactory : IAssetFactory
{
    private readonly DocumentInformation _info;

    public GlbAssetFactory(DocumentInformation? info = null)
    {
        _info = info ?? new()
        {
            Name = "GlTF of Model"
        };
    }

    /// <inheritdoc/>
    public IEnumerable<Task<Asset>> Execute(IAssetBuilderContext context)
    {
        string fileName = _info.Id + ".glb";
        string tempPath = Path.GetTempFileName();
        context.Model.ToGlTF(tempPath, out List<BaseError> errors);

        Asset asset = new()
        {
            Info = _info.CloneAs(),
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

        Task<Asset> task = context.StorageStrategy.WriteAsync(asset, fileName, stream);
        return new [] { task };
    }
}
