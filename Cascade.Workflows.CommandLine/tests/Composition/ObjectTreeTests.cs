using Cascade.Workflows.CommandLine.Composition;
using Cascade.Workflows.CommandLine.Tests.Resources;
using NUnit.Framework;
using StudioLE.Extensions.System;
using StudioLE.Diagnostics;
using StudioLE.Diagnostics.NUnit;
using StudioLE.Verify;

namespace Cascade.Workflows.CommandLine.Tests.Composition;

internal sealed class ObjectTreeTests
{
    private readonly IContext _context = new NUnitContext();

    [Test]
    public async Task ObjectTree_FlattenProperties()
    {
        // Arrange
        ExampleClass inputs = new();
        ObjectTree objectTree = new(inputs);

        // Act
        ObjectTreeProperty[] properties = objectTree
            .FlattenProperties()
            .ToArray();
        string[] output = properties
            .Select(x => $"{x.FullKey}: {x.Type}")
            .ToArray();

        // Assert
        await _context.Verify(output.Join());
    }

    [Test]
    public async Task ObjectTree_ValidateValue()
    {
        // Arrange
        ExampleClass inputs = new();
        ObjectTree objectTree = new(inputs);

        // Act
        ObjectTreeProperty[] properties = objectTree
            .FlattenProperties()
            .ToArray();
        string[] errors = properties
            .SelectMany(x => x.ValidateValue())
            .ToArray();

        // Assert
        await _context.Verify(errors.Join());
    }

    [TestCase("Hello, world!")]
    public async Task ObjectTree_SupportedTypes(object obj)
    {
        // Arrange
        ObjectTree objectTree = new(obj);

        // Act
        ObjectTreeProperty[] properties = objectTree
            .FlattenProperties()
            .ToArray();
        string[] output = properties
            .Select(x => $"{x.Type} {x.FullKey}: {x.GetValue()}")
            .ToArray();

        // Assert
        await _context.Verify(output.Join());
    }

    [Test]
    public async Task ObjectTreeProperty_SetValue_RecordStruct()
    {
        // Arrange
        ExampleRecordStruct obj = new()
        {
            RecordStructStringValue = "This is a string value.",
            RecordStructArgValue = "This is an argument value."
        };
        ObjectTree objectTree = new(obj);

        // Act
        objectTree.Properties.ElementAt(0).SetValue("This is a new string value.");
        ObjectTreeProperty[] properties = objectTree
            .FlattenProperties()
            .ToArray();
        string[] output = properties
            .Select(x => $"{x.Type} {x.FullKey}: {x.GetValue()}")
            .ToArray();

        // Assert
        await _context.Verify(output.Join());
    }

    [Test]
    public async Task ObjectTreeProperty_SetValue_NestedRecordStruct()
    {
        // Arrange
        ExampleClass inputs = new();
        ObjectTree objectTree = new(inputs);
        ObjectTreeProperty recordProperty = objectTree
            .Properties
            .First(x => x.Type == typeof(ExampleRecordStruct));

        // Act
        recordProperty.Properties.ElementAt(0).SetValue("This is a new string value.");
        ObjectTreeProperty[] properties = objectTree
            .FlattenProperties()
            .ToArray();
        string[] output = properties
            .Select(x => $"{x.Type} {x.FullKey}: {x.GetValue()}")
            .ToArray();

        // Assert
        await _context.Verify(output.Join());
    }
}
