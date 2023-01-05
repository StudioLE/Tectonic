using Lineweights.Core.Documents;

namespace Lineweights.Workflows.Documents;

public class AssetFactory : IAssetFactory
{
    private readonly IReadOnlyCollection<Asset> _assets;


    public AssetFactory(params Asset[] assets)
    {
        _assets = assets;
    }

    /// <inheritdoc/>
    public IEnumerable<Task<Asset>> Execute(IAssetBuilderContext context)
    {
        return _assets.Select(Task.FromResult);
    }
}
