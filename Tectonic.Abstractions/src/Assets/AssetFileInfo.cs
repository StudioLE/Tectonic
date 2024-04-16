namespace Tectonic.Assets;

/// <inheritdoc cref="IAssetFileInfo"/>
public readonly record struct AssetFileInfo : IAssetFileInfo
{
    /// <inheritdoc />
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <inheritdoc />
    public string Name { get; init; } = string.Empty;

    /// <inheritdoc />
    public string Description { get; init; } = string.Empty;

    /// <inheritdoc />
    public string MediaType { get; init; } = string.Empty;

    /// <inheritdoc />
    public string Location { get; init; } = string.Empty;

    /// <inheritdoc />
    public bool Exists { get; init; } = false;

    /// <inheritdoc />
    public IAssetContainerInfo? ContainerInformation { get; init; } = null;

    /// <summary>
    /// Create a new instance of <see cref="AssetFileInfo"/>.
    /// </summary>
    public AssetFileInfo()
    {
    }
}
