using NUnit.Framework;
using StudioLE.Core.Results;

namespace Geometrician.Core.Tests.Elements;

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
        IResult<Color> colorResult = element.GetProperty<Color>();
        IResult<ExampleClass> classResult = element.GetProperty<ExampleClass>();
        IResult<ExampleStruct> structResult = element.GetProperty<ExampleStruct>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(colorResult is Failure<Color>, "Color property is failure.");
            Assert.That(classResult is Success<ExampleClass>, "Class property is success");
            if (classResult is Success<ExampleClass> classSuccess)
            {
                Assert.That(classSuccess.Value.Example, Is.EqualTo(1), "Class value");
                ExampleClass classImplicit = classSuccess;
                Assert.That(classImplicit.Example, Is.EqualTo(1), "Class implicit value");
            }
            Assert.That(structResult is Success<ExampleStruct>, "Struct property is success");
            if (structResult is Success<ExampleStruct> structSuccess)
            {
                Assert.That(structSuccess.Value.Example, Is.EqualTo(1), "Struct value");
                ExampleStruct structImplicit = structSuccess;
                Assert.That(structImplicit.Example, Is.EqualTo(1), "Struct implicit value");
            }
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
        IResult<IEnumerable<Color>> colorResult = element.TryGetProperties<Color>();
        IResult<IEnumerable<ExampleClass>> classResult = element.TryGetProperties<ExampleClass>();
        IResult<IEnumerable<ExampleStruct>> structResult = element.TryGetProperties<ExampleStruct>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(colorResult is Failure<IEnumerable<Color>>, "Color property is failure.");
            Assert.That(classResult is Success<IEnumerable<ExampleClass>>, "Class property is success");
            if (classResult is Success<IEnumerable<ExampleClass>> classSuccess)
                Assert.That(classSuccess.Value.Count(), Is.EqualTo(2), "Class value");
            // IEnumerable<ExampleClass> classImplicit = classSuccess;
            // Assert.That(classImplicit, Is.EqualTo(1), "Class implicit value");
            Assert.That(structResult is Success<IEnumerable<ExampleStruct>>, "Struct property is success");
            if (structResult is Success<IEnumerable<ExampleStruct>> structSuccess)
                Assert.That(structSuccess.Value.Count(), Is.EqualTo(2), "Struct value");
            // IEnumerable<ExampleStruct> structImplicit = structSuccess;
            // Assert.That(structImplicit, Is.EqualTo(1), "Struct implicit value");
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
        public int Example { get; } = 1;
    }

    private struct ExampleStruct
    {
        public int Example { get; } = 1;

        public ExampleStruct()
        {
        }
    }
}
