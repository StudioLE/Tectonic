using Elements.Serialization.JSON;
using Lineweights.Core.Documents;
using Lineweights.Core.Serialisation;
using Lineweights.Workflows.NUnit.Verification;
using Newtonsoft.Json;

namespace Lineweights.Workflows.Tests;

internal sealed class SerialisationTests
{
    [Test]
    public void Serialisation_InternalAsset()
    {
        // Arrange
        JsonSerializerSettings settings = new()
        {
            ContractResolver = new IgnoreConverterResolver(typeof(JsonInheritanceConverter))
        };
        InternalAsset asset = new()
        {
            Name = "An example document",
            Description = "A description of the document.",
            ContentType = "text/plain",
            Content = "Hello, world!"
        };

        // Act
        string json = JsonConvert.SerializeObject(asset, settings);
        InternalAsset? deserialised = JsonConvert.DeserializeObject<InternalAsset>(json, settings);
        string json2 = JsonConvert.SerializeObject(deserialised, settings);

        // Assert
        Assert.Multiple(async () =>
        {
            await Verify.String(json, json2);
            Assert.That(deserialised, Is.Not.Null, "Not null");
            Assert.That(deserialised?.Id, Is.EqualTo(asset.Id), "Id");
            Assert.That(deserialised?.ContentType, Is.EqualTo(asset.ContentType), "ContentType");
            Assert.That(deserialised?.Content, Is.EqualTo(asset.Content), "Content");
        });
    }

    [Test]
    public void Serialisation_ExternalAsset()
    {
        // Arrange
        JsonSerializerSettings settings = new()
        {
            ContractResolver = new IgnoreConverterResolver(typeof(JsonInheritanceConverter))
        };
        ExternalAsset asset = new()
        {
            Name = "An example document",
            Description = "A description of the document.",
            ContentType = "text/plain",
            Location = new("https://localhost/my/file.txt")
        };

        // Act
        string json = JsonConvert.SerializeObject(asset, settings);
        ExternalAsset? deserialised = JsonConvert.DeserializeObject<ExternalAsset>(json, settings);
        string json2 = JsonConvert.SerializeObject(deserialised, settings);

        // Assert
        Assert.Multiple(async () =>
        {
            await Verify.String(json, json2);
            Assert.That(deserialised, Is.Not.Null, "Not null");
            Assert.That(deserialised?.Id, Is.EqualTo(asset.Id), "Id");
            Assert.That(deserialised?.ContentType, Is.EqualTo(asset.ContentType), "ContentType");
            Assert.That(deserialised?.Location, Is.EqualTo(asset.Location), "Location");
        });
    }

    [Test]
    public void Serialisation_InternalAsset_InModel()
    {
        // Arrange
        InternalAsset asset = new()
        {
            Name = "An example document",
            Description = "A description of the document.",
            ContentType = "text/plain",
            Content = "Hello, world!"
        };

        // Act
        // Assert
        VerifyHelpers.SerialisationAsModel(asset);
    }

    [Test]
    public void Serialisation_ExternalAsset_InModel()
    {
        // Arrange
        ExternalAsset asset = new()
        {
            Name = "An example document",
            Description = "A description of the document.",
            ContentType = "text/plain",
            Location = new("https://localhost/my/file.txt")
        };

        // Act
        // Assert
        VerifyHelpers.SerialisationAsModel(asset);
    }
}
