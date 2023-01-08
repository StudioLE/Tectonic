using StudioLE.Core.Conversion;

namespace Lineweights.Core.Documents;

/// <summary>
/// Convert a <see cref="Model"/> to a JSON file
/// referenced as <see cref="Asset"/>.
/// </summary>
public class ModelToJsonAsset : IConverter<Model, Task<Asset>>
{
    /// <inheritdoc />
    public Task<Asset> Convert(Model model)
    {
        string json = model.ToJson();
        Asset asset = new()
        {
            Info = new()
            {
                Name = "JSON of Model"
            },
            ContentType = "application/json",
            Content = json
        };
        return Task.FromResult(asset);
    }
}
