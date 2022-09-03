namespace Lineweights.Core.Documents;

/// <summary>
/// A <see href="https://refactoring.guru/design-patterns/builder">builder</see> to create an <see cref="Asset"/>.
/// </summary>
public interface IAssetBuilder
{
    /// <summary>
    /// Set the <see cref="IStorageStrategy"/>.
    /// </summary>
    public IAssetBuilder StorageStrategy(IStorageStrategy storage);

    /// <summary>
    /// Set the <see cref="Documents.DocumentInformation"/> of the parent <see cref="Asset"/>.
    /// </summary>
    public IAssetBuilder DocumentInformation(DocumentInformation doc);

    /// <summary>
    /// Add an asset to the builder.
    /// </summary>
    public IAssetBuilder AddAsset(Asset asset);

    /// <summary>
    /// Add a step to the builder.
    /// </summary>
    public IAssetBuilder AddStep(Step step);

    /// <summary>
    /// Add multiple steps to the builder.
    /// </summary>
    public IAssetBuilder AddSteps(Steps steps);

    /// <summary>
    /// Build asset.
    /// </summary>
    public Task<Asset> Build(Model model);

    public delegate Task<Asset> Step(Model model, IStorageStrategy storageStrategy);
    public delegate IEnumerable<Task<Asset>> Steps(Model model, IStorageStrategy storageStrategy);
}
