using System.IO;
using Geometrician.Core.Serialization;
using Geometrician.Diagnostics;
using Geometrician.Diagnostics.Samples;
using Newtonsoft.Json;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Geometrician.Core.Tests.Serialization;

internal sealed class SerializationTests
{
    private readonly Verify _verify = new(new NUnitVerifyContext());

    [Test]
    public async Task Serialization_FileInfoConverter()
    {
        // Arrange
        FileInfo expected = new(Path.GetTempFileName());

        // Act
        FileInfoConverter converter = new();
        string json = JsonConvert.SerializeObject(expected, converter);
        FileInfo? deserialised = JsonConvert.DeserializeObject<FileInfo>(json, converter);
        string json2 = JsonConvert.SerializeObject(expected, converter);

        // Assert
        await _verify.String(json, json2);
        Assert.That(deserialised, Is.Not.Null, "Not null");
        Assert.That(deserialised?.FullName, Is.EqualTo(expected.FullName), "FullName");
    }

    [Test]
    public async Task Serialization_TypeConverter()
    {
        // Arrange
        Type expected = typeof(HashCode);

        // Act
        TypeConverter converter = new();
        string json = JsonConvert.SerializeObject(expected, converter);
        Type? deserialised = JsonConvert.DeserializeObject<Type>(json, converter);
        string json2 = JsonConvert.SerializeObject(deserialised, converter);

        // Assert
        await _verify.String(json, json2);
        Assert.That(deserialised, Is.Not.Null, "Not null");
        Assert.That(deserialised?.FullName, Is.EqualTo(expected.FullName), "FullName");
        Assert.That(deserialised?.AssemblyQualifiedName, Is.EqualTo(expected.AssemblyQualifiedName), "AssemblyQualifiedName");
    }

    [TestCase(Scenes.Name.GeometricElements)]
    [TestCase(Scenes.Name.Brickwork)]
    public async Task Serialization_ModelConverter(Scenes.Name name)
    {
        // Arrange
        Model model = Scenes.FromJson(name);
        ModelConverter converter = new();

        // Act
        string json = JsonConvert.SerializeObject(model, converter);
        // await _verify.String(json);
        Model? deserialised = JsonConvert.DeserializeObject<Model>(json, converter);
        if(deserialised is null)
            throw new("Failed to deserialize.");
        string json2 = JsonConvert.SerializeObject(deserialised, converter);

        // Assert
        await _verify.String(json, json2);
        Assert.That(deserialised, Is.Not.Null, "Not null");
        await _verify.ByElementIds(model, deserialised);
    }
}
