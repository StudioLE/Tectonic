using Elements.Serialization.JSON;
using Lineweights.Core.Serialisation;
using Lineweights.Workflows.Assets;
using Newtonsoft.Json;

namespace Lineweights.Workflows.Tests;

internal sealed class SerialisationTests
{
    private readonly Model _model = new();

    [Test]
    public async Task Serialisation_Asset()
    {
        // Arrange
        Model model = new();
        model.AddElements(Scenes.GeometricElements());
        AssetBuilder builder = AssetBuilder.Default(new FileStorageStrategy(), model);

        Asset asset = await builder.Build();

        // Act
        string json = JsonConvert.SerializeObject(asset);
        Asset? deserialised = JsonConvert.DeserializeObject<Asset>(json);

        // Assert
        Assert.Multiple(() =>
        {
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
        DocumentInformation doc = new()
        {
            Name = "Example",
            Description = "Hello, world.",
            Location = new("https://localhost/my/file.txt")
        };
        JsonSerializerSettings settings = new()
        {
            ContractResolver = new IgnoreConverterResolver(typeof(JsonInheritanceConverter))
        };

        // Act
        string json = JsonConvert.SerializeObject(doc, settings);
        DocumentInformation? deserialised = JsonConvert.DeserializeObject<DocumentInformation>(json, settings);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(deserialised, Is.Not.Null, "Not null");
            Assert.That(deserialised?.Id, Is.EqualTo(doc.Id), "Id");
            Assert.That(deserialised?.Location, Is.EqualTo(doc.Location), "Location");
        });
    }
}
