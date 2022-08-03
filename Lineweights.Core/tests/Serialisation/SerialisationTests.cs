using System.IO;
using Lineweights.Core.Serialisation;
using Newtonsoft.Json;

namespace Lineweights.Core.Tests.Serialisation;

internal sealed class SerialisationTests
{
    [Test]
    public void Serialisation_FileInfoConverter()
    {
        // Arrange
        FileInfo expected = new(Path.GetTempFileName());

        // Act
        FileInfoConverter converter = new();
        string json = JsonConvert.SerializeObject(expected, converter);
        FileInfo? deserialised = JsonConvert.DeserializeObject<FileInfo>(json, converter);

        // Assert
        Assert.That(deserialised, Is.Not.Null, "Not null");
        Assert.That(deserialised?.FullName, Is.EqualTo(expected.FullName), "FullName");
    }

    [Test]
    public void Serialisation_TypeConverter()
    {
        // Arrange
        Type expected = typeof(HashCode);

        // Act
        TypeConverter converter = new();
        string json = JsonConvert.SerializeObject(expected, converter);
        Type? deserialised = JsonConvert.DeserializeObject<Type>(json, converter);

        // Assert
        Assert.That(deserialised, Is.Not.Null, "Not null");
        Assert.That(deserialised?.FullName, Is.EqualTo(expected.FullName), "FullName");
        Assert.That(deserialised?.AssemblyQualifiedName, Is.EqualTo(expected.AssemblyQualifiedName), "AssemblyQualifiedName");
    }
}
