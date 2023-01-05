using System.Collections.ObjectModel;
using Lineweights.Core.Documents;

namespace Lineweights.Workflows.Documents;

/// <inheritdoc cref="IAssetBuilder"/>
public class AssetBuilder : IAssetBuilder
{
    private DocumentInformation? _doc;

    /// <inheritdoc/>
    public IStorageStrategy StorageStrategy { get; set; } = new FileStorageStrategy();

    public Collection<IAssetFactory> Factories { get; } = new();

    /// <inheritdoc cref="StorageStrategy"/>
    public AssetBuilder SetStorageStrategy(IStorageStrategy storageStrategy)
    {
        StorageStrategy = storageStrategy;
        return this;
    }

    /// <summary>
    /// Set the <see cref="SetDocumentInformation"/> of the parent <see cref="Asset"/>.
    /// </summary>
    public AssetBuilder SetDocumentInformation(DocumentInformation doc)
    {
        _doc = doc;
        return this;
    }

    /// <summary>
    /// Build asset.
    /// </summary>
    public async Task<Asset> Build(Model model)
    {
        if (!Factories.Any())
            SetDefaultTasks();
        AssetBuilderContext context = new()
        {
            Model = model,
            StorageStrategy = StorageStrategy
        };
        // TODO: Benchmark whether it's worth using tasks.AsParallel()
        IEnumerable<Task<Asset>> tasks = Factories
            .SelectMany(factory => factory.Execute(context));
        Asset[] assets = await Task.WhenAll(tasks);
        return new()
        {
            Info = _doc ?? new(),
            Children = assets
        };
    }

    private void SetDefaultTasks()
    {
        this.ConvertModelToGlb();
    }
}
