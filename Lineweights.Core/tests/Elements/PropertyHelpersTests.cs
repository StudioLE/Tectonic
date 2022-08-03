using Ardalis.Result;

namespace Lineweights.Core.Tests.Elements;

internal sealed class PropertyHelpersTests
{
    [Test]
    public void PropertyHelpers_TryGetProperty_ByType()
    {
        // Arrange
        GeometricElement element = new();
        string classKey = typeof(ExampleClass).FullName!;
        string structKey = typeof(ExampleStruct).FullName!;
        element.AdditionalProperties[classKey] = new ExampleClass();
        element.AdditionalProperties[structKey] = new ExampleStruct();

        // Act
        Result<Color> colorResult = element.GetProperty<Color>();
        Result<ExampleClass> classResult = element.GetProperty<ExampleClass>();
        Result<ExampleStruct> structResult = element.GetProperty<ExampleStruct>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(colorResult.Status, Is.EqualTo(ResultStatus.NotFound), "Color property status.");
            Assert.That(colorResult.Value, Is.EqualTo(default(Color)), "Color value");
            Assert.That(classResult.Status, Is.EqualTo(ResultStatus.Ok), "Class property status");
            Assert.That(classResult.Value, Is.Not.Null, "Class value");
            Assert.That(classResult.Value.Example, Is.EqualTo(1), "Class value");
            Assert.That(structResult.Status, Is.EqualTo(ResultStatus.Ok), "Struct property exists");
            Assert.That(structResult.Value, Is.Not.Null, "Struct value");
            Assert.That(structResult.Value.Example, Is.EqualTo(1), "Struct value");
        });
    }

    [Test]
    public void PropertyHelpers_SetProperty_ByType()
    {
        // Arrange
        GeometricElement element = new();
        string key = typeof(ExampleClass).FullName!;
        element.AdditionalProperties[key] = new();
        string classKey = typeof(ExampleClass).FullName!;
        string structKey = typeof(ExampleStruct).FullName!;

        // Act
        element.SetProperty(new ExampleClass());
        element.SetProperty(new ExampleStruct());

        // Assert
        Assert.Multiple(() =>
        {
            bool classKeyExists = element.AdditionalProperties.TryGetValue(classKey, out object? classValue);
            Assert.That(classKeyExists, "Class key exists");
            Assert.That(classValue, Is.Not.Null, "Class key value");
            Assert.That(classValue is ExampleClass, "Class value type");
            bool structKeyExists = element.AdditionalProperties.TryGetValue(structKey, out object? structValue);
            Assert.That(structKeyExists, "Struct key exists");
            Assert.That(structValue, Is.Not.Null, "Struct value");
            Assert.That(structValue is ExampleStruct, "Struct value type");
        });
    }
    [Test]
    public void PropertyHelpers_TryGetProperties_ByType()
    {
        // Arrange
        GeometricElement element = new();
        string classKey = typeof(IEnumerable<ExampleClass>).FullName!;
        string structKey = typeof(IEnumerable<ExampleStruct>).FullName!;
        element.AdditionalProperties[classKey] = new ExampleClass[]
        {
            new(),
            new()
        };
        element.AdditionalProperties[structKey] = new ExampleStruct[]
        {
            new(),
            new()
        };

        // Act
        Result<IEnumerable<Color>> colorResult = element.TryGetProperties<Color>();
        Result<IEnumerable<ExampleClass>> classResult = element.TryGetProperties<ExampleClass>();
        Result<IEnumerable<ExampleStruct>> structResult = element.TryGetProperties<ExampleStruct>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(colorResult.Status, Is.EqualTo(ResultStatus.NotFound), "Color property status.");
            Assert.That(colorResult.Value, Is.Null, "Color value");
            Assert.That(classResult.Status, Is.EqualTo(ResultStatus.Ok), "Class property status");
            Assert.That(classResult.Value, Is.Not.Null, "Class value");
            Assert.That(classResult.Value.Count(), Is.EqualTo(2), "Class value");
            Assert.That(structResult.Status, Is.EqualTo(ResultStatus.Ok), "Struct property exists");
            Assert.That(structResult.Value, Is.Not.Null, "Struct value");
            Assert.That(structResult.Value.Count(), Is.EqualTo(2), "Struct value");
        });
    }

    [Test]
    public void PropertyHelpers_SetProperties_ByType()
    {
        // Arrange
        GeometricElement element = new();
        string key = typeof(ExampleClass).FullName!;
        element.AdditionalProperties[key] = new();
        string classKey = typeof(IEnumerable<ExampleClass>).FullName!;
        string structKey = typeof(IEnumerable<ExampleStruct>).FullName!;

        // Act
        element.SetProperties(new ExampleClass[]
        {
            new(),
            new()
        });
        element.SetProperties(new ExampleStruct[]
        {
            new(),
            new()
        });

        // Assert
        Assert.Multiple(() =>
        {
            bool classKeyExists = element.AdditionalProperties.TryGetValue(classKey, out object? classValue);
            Assert.That(classKeyExists, "Class key exists");
            Assert.That(classValue, Is.Not.Null, "Class key value");
            Assert.That(classValue is IEnumerable<ExampleClass>, "Class value type");
            bool structKeyExists = element.AdditionalProperties.TryGetValue(structKey, out object? structValue);
            Assert.That(structKeyExists, "Struct key exists");
            Assert.That(structValue, Is.Not.Null, "Struct value");
            Assert.That(structValue is IEnumerable<ExampleStruct>, "Struct value type");
        });
    }

    private class ExampleClass
    {
        public int Example { get; set; } = 1;
    }

    private struct ExampleStruct
    {
        public int Example { get; set; } = 1;

        public ExampleStruct()
        {
        }
    }

}
