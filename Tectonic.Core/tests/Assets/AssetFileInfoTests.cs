using Newtonsoft.Json;
using NUnit.Framework;
using StudioLE.Diagnostics;
using StudioLE.Diagnostics.NUnit;
using StudioLE.Extensions.System;
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

    [TestCase("https://john.doe@www.example.com:123/forum/questions/?tag=networking&order=newest#top", false)]
    [TestCase("mailto:John.Doe@example.com", false)]
    [TestCase("tel:+1-816-555-1212", false)]
    [TestCase("file://localhost/c:/WINDOWS/clock.avi", false)]
    [TestCase("file:///c:/WINDOWS/clock.avi", false)]
    [TestCase("urn:oasis:names:specification:docbook:dtd:xml:4.1.2", false)]
    [TestCase("file://localhost/etc/fstab", false)]
    [TestCase("file:///etc/fstab", false)]
    [TestCase("file:/etc/fstab", false)]
    [TestCase("data:image/jpeg;base64,/9j/4AAQSkZJRgABAgAAZABkAAD", true)]
    public void AssetFileInfoHelpers_IsInlineData(string location, bool expected)
    {
        // Arrange
        AssetFileInfo asset = new()
        {
            Location = location
        };

        // Act
        bool result = asset.IsInlineData();

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("https://john.doe@www.example.com:123/forum/questions/?tag=networking&order=newest#top", false)]
    [TestCase("mailto:John.Doe@example.com", false)]
    [TestCase("tel:+1-816-555-1212", false)]
    [TestCase("file://localhost/c:/WINDOWS/clock.avi", true)]
    [TestCase("file:///c:/WINDOWS/clock.avi", true)]
    [TestCase("urn:oasis:names:specification:docbook:dtd:xml:4.1.2", false)]
    [TestCase("file://localhost/etc/fstab", true)]
    [TestCase("file:///etc/fstab", true)]
    [TestCase("file:/etc/fstab", true)]
    public void AssetFileInfoHelpers_IsLocalFile(string location, bool expected)
    {
        // Arrange
        AssetFileInfo asset = new()
        {
            Location = location
        };

        // Act
        bool result = asset.IsLocalFile();

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void AssetFileInfoHelpers_GetInlineData_UTF8()
    {
        // Arrange
        AssetFileInfo asset = new()
        {
            Location = "data:text/plain;charset=UTF-8;page=21,the%20data:1234,5678"
        };

        // Act
        string result = asset.GetInlineData();

        // Assert
        Assert.That(result, Is.EqualTo("the%20data:12345678"));
    }

    [Test]
    public async Task AssetFileInfoHelpers_GetInlineData_Svg()
    {
        // Arrange
        AssetFileInfo asset = new()
        {
            Location = "data:image/svg+xml;utf8,\n<svg width='10' height='10' xmlns='http://www.w3.org/2000/svg'>\n <circle style='fill:red' cx='5' cy='5' r='5'/>\n</svg>"
        };

        // Act
        string result = asset.GetInlineData();

        // Assert
        await _context.Verify(result);
    }

    [Test]
    public async Task AssetFileInfoHelpers_GetInlineDataBytes_Base64()
    {
        // Arrange
        AssetFileInfo asset = new()
        {
            Location = "data:image/png;base64,iVBORw0KGgoAAA\nANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4\n//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU\n5ErkJggg=="
        };

        // Act
        byte[] result = asset.GetInlineDataBytes();

        // Assert
        await _context.Verify(result.Select(x => x.ToString()).Join());
    }


}
