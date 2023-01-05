using Lineweights.Core.Documents;
using StudioLE.Core.System;

namespace Lineweights.Workflows.Documents;

public class JsonAssetFactory : IAssetFactory
{
    private readonly DocumentInformation _info;

    public JsonAssetFactory(DocumentInformation? info = null)
    {
        _info = info ?? new()
        {
            Name = "JSON of Model"
        };
    }

    /// <inheritdoc/>
    public IEnumerable<Task<Asset>> Execute(IAssetBuilderContext context)
    {
        string json = context.Model.ToJson();
        Asset asset = new()
        {
            Info = _info.CloneAs(),
            ContentType = "application/json",
            Content = json
        };
        return new [] { Task.FromResult(asset) };
    }
}
