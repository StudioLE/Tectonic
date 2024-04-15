namespace Tectonic.Assets;

/// <summary>
/// A uniquely identified document and its <see cref="Content"/>.
/// </summary>
/// <inheritdoc cref="IAsset"/>
public readonly record struct InternalAsset : IAsset
{
    /// <inheritdoc/>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <inheritdoc/>
    public string Name { get; init; } = string.Empty;

    /// <inheritdoc/>
    public string Description { get; init; } = string.Empty;

    /// <inheritdoc/>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>
    /// The content of the asset.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Create a new instance of <see cref="InternalAsset"/>.
    /// </summary>
    public InternalAsset()
    {
    }
}
