using Lineweights.Core.Serialisation;
using Newtonsoft.Json;

namespace Lineweights.Workflows.Results;

/// <summary>
/// The composite results of a workflow.
/// </summary>
public sealed class Result
{
    /// <summary>
    /// The document doc.
    /// </summary>
    [JsonConverter(typeof(OverrideInheritanceConverter))]
    public DocumentInformation Metadata { get; set; } = new();

    /// <summary>
    /// The uri of any additional files.
    /// </summary>
    public IReadOnlyCollection<Result> Children { get; set; } = Array.Empty<Result>();

    /// <summary>
    /// The title of the tile.
    /// </summary>
    public IReadOnlyCollection<string> Errors { get; set; } = Array.Empty<string>();

    /// <inheritdoc cref="Result"/>
    internal Result()
    {
    }
}
