using Newtonsoft.Json;
using NUnit.Framework;
using StudioLE.Diagnostics;
using StudioLE.Diagnostics.NUnit;
using StudioLE.Verify;
using Tectonic.Assets;

namespace Tectonic.Core.Tests.Assets;

internal sealed class AssetFileInfoTests
{
    private readonly IContext _context = new NUnitContext();

    [Test]
    public void AssetFileInfo_Serialization_Newtonsoft()
    {
        // Arrange
        JsonSerializerSettings settings = new();
        AssetFileInfo asset = new()
        {
            Name = "An example document",
            Description = "A description of the document.",
            MediaType = "text/plain",
            Location = "https://localhost/my/file.txt"
        };

        // Act
        string json = JsonConvert.SerializeObject(asset, settings);
        IAssetFileInfo? deserialized = JsonConvert.DeserializeObject<AssetFileInfo>(json, settings);
        string json2 = JsonConvert.SerializeObject(deserialized, settings);

        // Assert
        Assert.Multiple(async () =>
        {
            await _context.Verify(json, json2);
            Assert.That(deserialized, Is.Not.Null, "Not null");
            Assert.That(deserialized?.Id, Is.EqualTo(asset.Id), "Id");
            Assert.That(deserialized?.Name, Is.EqualTo(asset.Name), "Name");
            Assert.That(deserialized?.Description, Is.EqualTo(asset.Description), "Description");
            Assert.That(deserialized?.MediaType, Is.EqualTo(asset.MediaType), "MediaType");
            Assert.That(deserialized?.Location, Is.EqualTo(asset.Location), "Location");
        });
    }
}
