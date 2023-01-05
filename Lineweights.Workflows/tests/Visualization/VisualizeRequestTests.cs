using Lineweights.Core.Documents;
using Lineweights.Workflows.Documents;
using Lineweights.Workflows.Visualization;
using Newtonsoft.Json;

namespace Lineweights.Workflows.Tests.Visualization;

internal sealed class VisualizeRequestTests
{
    [TestCase(Scenes.Name.GeometricElements)]
    [TestCase(Scenes.Name.Brickwork)]
    public async Task VisualizeRequest_Serialize(Scenes.Name name)
    {
        // Arrange
        Model model = Scenes.FromJson(name);
        VisualizeRequestConverter converter = new();
        AssetBuilder assetBuilder = new();
        Asset asset = await assetBuilder.Build(model);
        VisualizeRequest request = new()
        {
            Model = model,
            Asset = asset
        };

        // Act
        string json = JsonConvert.SerializeObject(request, converter);
        // await Verify.String(json);
        VisualizeRequest? deserialised = JsonConvert.DeserializeObject<VisualizeRequest>(json, converter);
        string json2 = JsonConvert.SerializeObject(deserialised, converter);

        // Assert
        await Verify.String(json, json2);
        Assert.That(deserialised, Is.Not.Null, "Not null");
        Assert.That(deserialised?.Asset.Info.Id, Is.EqualTo(asset.Info.Id), "Asset Id");
        await Verify.ByElementIds(model, deserialised!.Model);
    }
}
