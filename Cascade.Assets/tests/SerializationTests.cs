using Elements.Serialization.JSON;
using Geometrician.Assets;
using Geometrician.Serialization.Json;
using Newtonsoft.Json;
using NUnit.Framework;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;
using VerifyHelpers = Geometrician.Diagnostics.NUnit.VerifyHelpers;

namespace Cascade.Assets.Tests;

internal sealed class SerializationTests
{
    private readonly IVerify _verify = new NUnitVerify();

    [Test]
    public void Serialization_InternalAsset()
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
        InternalAsset? deserialized = JsonConvert.DeserializeObject<InternalAsset>(json, settings);
        string json2 = JsonConvert.SerializeObject(deserialized, settings);

        // Assert
        Assert.Multiple(async () =>
        {
            await _verify.String(json, json2);
            Assert.That(deserialized, Is.Not.Null, "Not null");
            Assert.That(deserialized?.Id, Is.EqualTo(asset.Id), "Id");
            Assert.That(deserialized?.ContentType, Is.EqualTo(asset.ContentType), "ContentType");
            Assert.That(deserialized?.Content, Is.EqualTo(asset.Content), "Content");
        });
    }

    [Test]
    public void Serialization_ExternalAsset()
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
        ExternalAsset? deserialized = JsonConvert.DeserializeObject<ExternalAsset>(json, settings);
        string json2 = JsonConvert.SerializeObject(deserialized, settings);

        // Assert
        Assert.Multiple(async () =>
        {
            await _verify.String(json, json2);
            Assert.That(deserialized, Is.Not.Null, "Not null");
            Assert.That(deserialized?.Id, Is.EqualTo(asset.Id), "Id");
            Assert.That(deserialized?.ContentType, Is.EqualTo(asset.ContentType), "ContentType");
            Assert.That(deserialized?.Location, Is.EqualTo(asset.Location), "Location");
        });
    }

    [Test]
    public void Serialization_InternalAsset_InModel()
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
       VerifyHelpers.SerializationAsModel(asset);
    }

    [Test]
    public void Serialization_ExternalAsset_InModel()
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
        VerifyHelpers.SerializationAsModel(asset);
    }
}
