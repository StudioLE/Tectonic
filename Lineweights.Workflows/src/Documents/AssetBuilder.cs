using Lineweights.Core.Documents;

namespace Lineweights.Workflows.Documents;

/// <inheritdoc cref="IAssetBuilder"/>
public class AssetBuilder : IAssetBuilder
{
    private readonly List<IAssetBuilder.Steps> _steps = new();
    private IStorageStrategy _storageStrategy = new FileStorageStrategy();
    private DocumentInformation? _doc;

    /// <inheritdoc />
    public IAssetBuilder StorageStrategy(IStorageStrategy storage)
    {
        _storageStrategy = storage;
        return this;
    }

    /// <inheritdoc />
    public IAssetBuilder AddAsset(Asset asset)
    {
        AddStep((_, _) => Task.FromResult(asset));
        return this;
    }

    /// <inheritdoc />
    public IAssetBuilder AddStep(IAssetBuilder.Step step)
    {
        AddSteps((model, storageStrategy) =>
        {
            Task<Asset> task = step.Invoke(model, storageStrategy);
            return new[] { task };
        });
        return this;
    }

    /// <inheritdoc />
    public IAssetBuilder AddSteps(IAssetBuilder.Steps steps)
    {
        _steps.Add(steps);
        return this;
    }

    /// <inheritdoc />
    public IAssetBuilder DocumentInformation(DocumentInformation doc)
    {
        _doc = doc;
        return this;
    }

    /// <inheritdoc cref="Asset"/>
    public async Task<Asset> Build(Model model)
    {
        if (!_steps.Any())
            this.ConvertModelToGlb();
        // TODO: Benchmark whether it's worth using tasks.AsParallel()
        IEnumerable<Task<Asset>> tasks = _steps.SelectMany(steps => steps.Invoke(model, _storageStrategy));
        Asset[] assets = await Task.WhenAll(tasks);
        return new()
        {
            Info = _doc ?? new(),
            Children = assets
        };
    }
}
