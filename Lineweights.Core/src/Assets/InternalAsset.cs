using System.IO;
using System.Text;
using Lineweights.Core.Storage;
using StudioLE.Core.Results;
using StudioLE.Core.System.IO;

namespace Lineweights.Core.Assets;

/// <inheritdoc cref="IAsset"/>
public class InternalAsset : Element, IAsset, ICloneable
{
    /// <inheritdoc />
    public string Description { get; set; } = string.Empty;

    /// <inheritdoc />
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// The content of the asset.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <inheritdoc />
    public object Clone()
    {
        return new InternalAsset
        {
            Id = Id,
            Name = Name,
            AdditionalProperties = new Dictionary<string, object>(AdditionalProperties),
            Description = Description,
            ContentType = ContentType,
            Content = Content
        };
    }

    /// <summary>
    /// Convert an <see cref="InternalAsset"/> to an <see cref="ExternalAsset"/> by writing the content to
    /// <see cref="IStorageStrategy"/>.
    /// </summary>
    public async Task<IResult<ExternalAsset>> ToExternalAsset(IStorageStrategy storageStrategy, string? fileName)
    {
        fileName ??= Id + (ContentType.GetExtensionByContentType() ?? ".txt");
        byte[] byteArray = Encoding.ASCII.GetBytes(Content);
        MemoryStream stream = new(byteArray);
        IResult<Uri> result = await storageStrategy.WriteAsync(fileName, stream);

        if (result is not Success<Uri> success)
            return new Failure<ExternalAsset>("Failed to write to storage.", result.Errors)
            {
                Warnings = result.Warnings
            };

        return new Success<ExternalAsset>(new()
        {
            Id = Id,
            Name = Name,
            AdditionalProperties = new Dictionary<string, object>(AdditionalProperties),
            Description = Description,
            ContentType = ContentType,
            Location = success.Value
        });
    }
}
