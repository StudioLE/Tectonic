namespace Tectonic.Assets;

/// <summary>
/// A uniquely identified document and its <see cref="Content"/>.
/// </summary>
/// <inheritdoc cref="IAsset"/>
public record struct InternalAsset : IAsset
{
    /// <inheritdoc/>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc/>
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string Description { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// The content of the asset.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Create a new instance of <see cref="InternalAsset"/>.
    /// </summary>
    public InternalAsset()
    {
    }
}
