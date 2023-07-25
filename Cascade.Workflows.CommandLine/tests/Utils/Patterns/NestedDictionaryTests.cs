using Cascade.Workflows.CommandLine.Utils.Patterns;
using NUnit.Framework;

namespace Cascade.Workflows.CommandLine.Tests.Utils.Patterns;

internal sealed class NestedDictionaryTests
{
    [Test]
    public void NestedDictionary_Simple()
    {
        // Arrange
        NestedDictionary<string, string> dictionary = new();
        string[] keys = { "a1", "a2", "a3" };
        const string value = "Hello, world!";
        dictionary.SetByKeys(keys, value);

        // Act
        NestedDictionary<string, string>? actual = dictionary.GetByKeys(keys);

        // Assert
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual!.Value, Is.EqualTo(value));
    }

    [Test]
    public void NestedDictionary_Complex()
    {
        // Arrange
        NestedDictionary<string, string> dictionary = new();
        string[] keysA = { "a0", "a1", "a2" };
        string[] keysB = { "b0", "b1", "b3" };
        string[] keysAa = { "a0", "aa1", "aa2" };
        const string value = "Hello, world!";
        dictionary.SetByKeys(keysA, value);
        dictionary.SetByKeys(keysB, value);
        dictionary.SetByKeys(keysAa, value);

        // Act
        NestedDictionary<string, string>? actual = dictionary.GetByKeys(keysAa);

        // Assert
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual!.Value, Is.EqualTo(value));
    }
}
