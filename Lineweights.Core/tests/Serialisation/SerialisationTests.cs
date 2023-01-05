using System.IO;
using Lineweights.Core.Serialisation;
using Newtonsoft.Json;

namespace Lineweights.Core.Tests.Serialisation;

internal sealed class SerialisationTests
{
    [Test]
    public async Task Serialisation_FileInfoConverter()
    {
        // Arrange
        FileInfo expected = new(Path.GetTempFileName());

        // Act
        FileInfoConverter converter = new();
        string json = JsonConvert.SerializeObject(expected, converter);
        FileInfo? deserialised = JsonConvert.DeserializeObject<FileInfo>(json, converter);
        string json2 = JsonConvert.SerializeObject(expected, converter);

        // Assert
        await Verify.String(json, json2);
        Assert.That(deserialised, Is.Not.Null, "Not null");
        Assert.That(deserialised?.FullName, Is.EqualTo(expected.FullName), "FullName");
    }

    [Test]
    public async Task Serialisation_TypeConverter()
    {
        // Arrange
        Type expected = typeof(HashCode);

        // Act
        TypeConverter converter = new();
        string json = JsonConvert.SerializeObject(expected, converter);
        Type? deserialised = JsonConvert.DeserializeObject<Type>(json, converter);
        string json2 = JsonConvert.SerializeObject(deserialised, converter);

        // Assert
        await Verify.String(json, json2);
        Assert.That(deserialised, Is.Not.Null, "Not null");
        Assert.That(deserialised?.FullName, Is.EqualTo(expected.FullName), "FullName");
        Assert.That(deserialised?.AssemblyQualifiedName, Is.EqualTo(expected.AssemblyQualifiedName), "AssemblyQualifiedName");
    }

    [TestCase(Scenes.Name.GeometricElements)]
    [TestCase(Scenes.Name.Brickwork)]
    public async Task Serialisation_ModelConverter(Scenes.Name name)
    {
        // Arrange
        Model model = Scenes.FromJson(name);
        ModelConverter converter = new();

        // Act
        string json = JsonConvert.SerializeObject(model, converter);
        // await Verify.String(json);
        Model? deserialised = JsonConvert.DeserializeObject<Model>(json, converter);
        string json2 = JsonConvert.SerializeObject(deserialised, converter);

        // Assert
        await Verify.String(json, json2);
        Assert.That(deserialised, Is.Not.Null, "Not null");
        await Verify.ByElementIds(model, deserialised);
    }
}
