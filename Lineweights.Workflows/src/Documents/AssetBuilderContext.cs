using Lineweights.Core.Documents;

namespace Lineweights.Workflows.Documents;

public class AssetBuilderContext : IAssetBuilderContext
{
    /// <inheritdoc/>
    public IStorageStrategy StorageStrategy { get; set; } = new FileStorageStrategy();

    /// <inheritdoc/>
    public Model Model { get; set; } = new();
}
