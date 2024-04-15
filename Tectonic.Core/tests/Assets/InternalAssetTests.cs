using Newtonsoft.Json;
using NUnit.Framework;
using StudioLE.Diagnostics;
using StudioLE.Diagnostics.NUnit;
using StudioLE.Verify;
using Tectonic.Assets;

namespace Tectonic.Core.Tests.Assets;

internal sealed class InternalAssetTests
{
    private readonly IContext _context = new NUnitContext();

    [Test]
    public void InternalAsset_Serialization_Newtonsoft()
    {
        // Arrange
        JsonSerializerSettings settings = new();
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
            await _context.Verify(json, json2);
            Assert.That(deserialized, Is.Not.Null, "Not null");
            Assert.That(deserialized?.Id, Is.EqualTo(asset.Id), "Id");
            Assert.That(deserialized?.Name, Is.EqualTo(asset.Name), "Name");
            Assert.That(deserialized?.Description, Is.EqualTo(asset.Description), "Description");
            Assert.That(deserialized?.ContentType, Is.EqualTo(asset.ContentType), "ContentType");
            Assert.That(deserialized?.Content, Is.EqualTo(asset.Content), "Content");
        });
    }
}
