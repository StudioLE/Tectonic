using Geometrician.Core.Visualization;
using Lineweights.Diagnostics.Samples;
using Newtonsoft.Json;

namespace Lineweights.Diagnostics.Tests.Visualization;

internal sealed class VisualizeRequestTests
{
    [TestCase(Scenes.Name.GeometricElements)]
    [TestCase(Scenes.Name.Brickwork)]
    public async Task VisualizeRequest_Serialize(Scenes.Name name)
    {
        // Arrange
        Model model = Scenes.FromJson(name);
        VisualizeRequestConverter converter = new();
        VisualizeRequest request = new()
        {
            Model = model
        };

        // Act
        string json = JsonConvert.SerializeObject(request, converter);
        // await Verify.String(json);
        VisualizeRequest? deserialised = JsonConvert.DeserializeObject<VisualizeRequest>(json, converter);
        string json2 = JsonConvert.SerializeObject(deserialised, converter);

        // Assert
        await Verify.String(json, json2);
        Assert.That(deserialised, Is.Not.Null, "Not null");
        await Verify.ByElementIds(model, deserialised!.Model);
    }
}
