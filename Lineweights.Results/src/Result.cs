namespace Lineweights.Results;

/// <summary>
/// The composite results of a workflow.
/// </summary>
public sealed class Result
{
    /// <summary>
    /// The document metadata.
    /// </summary>
    public DocumentInformation Metadata { get; internal set; } = new();

    /// <summary>
    /// The uri of any additional files.
    /// </summary>
    public ICollection<Result> Children { get; } = new List<Result>();

    /// <summary>
    /// The title of the tile.
    /// </summary>
    public IReadOnlyCollection<string> Errors { get; internal set; } = Array.Empty<string>();

    /// <inheritdoc cref="Result"/>
    internal Result()
    {
    }
}
