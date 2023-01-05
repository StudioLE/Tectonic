using Elements.Serialization.JSON;
using Lineweights.Core.Documents;
using Lineweights.Core.Serialisation;
using Lineweights.Workflows.Documents;
using Lineweights.Workflows.Hosting;
using Lineweights.Workflows.NUnit.Verification;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Lineweights.Workflows.Tests;

internal sealed class SerialisationTests
{
    private readonly AssetBuilder _builder;

    private static readonly DocumentInformation _doc = new()
    {
        Name = "Example",
        Description = "Hello, world.",
        Location = new("https://localhost/my/file.txt")
    };

    public SerialisationTests()
    {
        IServiceProvider services = Services.GetInstance();
        _builder = (AssetBuilder)services.GetRequiredService<IAssetBuilder>();
    }

    [Test]
    public async Task Serialisation_Asset()
    {
        // Arrange
        Model model = new();
        model.AddElements(Scenes.GeometricElements());
        _builder.SetDocumentInformation(_doc);
        Asset asset = await _builder.Build(model);

        // Act
        string json = JsonConvert.SerializeObject(asset);
        Asset? deserialised = JsonConvert.DeserializeObject<Asset>(json);
        string json2 = JsonConvert.SerializeObject(deserialised);

        // Assert
        Assert.Multiple(async () =>
        {
            await Verify.String(json, json2);
            Assert.That(deserialised, Is.Not.Null, "Not null");
            Assert.That(deserialised?.Info.Id, Is.EqualTo(asset.Info.Id), "Parent Id");
            Asset? glb = asset.Children.FirstOrDefault();
            Asset? deserialisedGlb = deserialised?.Children.FirstOrDefault();
            Assert.That(deserialisedGlb, Is.Not.Null, "Child not null");
            Assert.That(deserialisedGlb?.Info.Id, Is.EqualTo(glb?.Info.Id), "Child Id");
        });
    }

    [Test]
    public void Serialisation_DocumentInformation()
    {
        // Arrange
        JsonSerializerSettings settings = new()
        {
            ContractResolver = new IgnoreConverterResolver(typeof(JsonInheritanceConverter))
        };

        // Act
        string json = JsonConvert.SerializeObject(_doc, settings);
        DocumentInformation? deserialised = JsonConvert.DeserializeObject<DocumentInformation>(json, settings);
        string json2 = JsonConvert.SerializeObject(deserialised, settings);

        // Assert
        Assert.Multiple(async () =>
        {
            await Verify.String(json, json2);
            Assert.That(deserialised, Is.Not.Null, "Not null");
            Assert.That(deserialised?.Id, Is.EqualTo(_doc.Id), "Id");
            Assert.That(deserialised?.Location, Is.EqualTo(_doc.Location), "Location");
        });
    }

    [Test]
    public void Serialisation_DocumentInformation_InModel()
    {
        // Arrange
        // Act
        // Assert
        VerifyHelpers.SerialisationAsModel(_doc);
    }
}
