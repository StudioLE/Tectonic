using Geometrician.Core.Visualization;
using Lineweights.Diagnostics.Samples;
using Newtonsoft.Json;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Lineweights.Diagnostics.Tests.Visualization;

internal sealed class VisualizeRequestTests
{
    private readonly Verify _verify = new(new NUnitVerifyContext());

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
        // await _verify.String(json);
        VisualizeRequest? deserialised = JsonConvert.DeserializeObject<VisualizeRequest>(json, converter);
        string json2 = JsonConvert.SerializeObject(deserialised, converter);

        // Assert
        await _verify.String(json, json2);
        Assert.That(deserialised, Is.Not.Null, "Not null");
        await _verify.ByElementIds(model, deserialised!.Model);
    }
}
