using Elements.Serialization.JSON;
using Lineweights.Core.Serialisation;
using Lineweights.Workflows.Results;
using Newtonsoft.Json;

namespace Lineweights.Workflows.Tests;

internal sealed class SerialisationTests : ResultModel
{
    [Test]
    public void Serialisation_Result()
    {
        // Arrange
        Model model = new();
        model.AddElements(Scenes.GeometricElements());

        Result result = ResultBuilder.Default(new FileStorageStrategy(), model);

        // Act
        string json = JsonConvert.SerializeObject(result);
        Result? deserialised = JsonConvert.DeserializeObject<Result>(json);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(deserialised, Is.Not.Null, "Not null");
            Assert.That(deserialised?.Info.Id, Is.EqualTo(result.Info.Id), "Parent Id");
            Result? glb = result.Children.FirstOrDefault();
            Result? deserialisedGlb = deserialised?.Children.FirstOrDefault();
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
