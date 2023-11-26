using Cascade.Workflows.CommandLine.Composition;
using Cascade.Workflows.CommandLine.Tests.Resources;
using NUnit.Framework;
using StudioLE.Extensions.System;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Cascade.Workflows.CommandLine.Tests.Composition;

internal sealed class ObjectTreeTests
{
    private readonly IVerify _verify = new NUnitVerify();

    [Test]
    public async Task ObjectTree_FlattenProperties()
    {
        // Arrange
        ObjectTree objectTree = ObjectTree.Create<ExampleClass>();

        // Act
        ObjectTreeProperty[] properties = objectTree.FlattenProperties().ToArray();
        string[] output = properties.Select(x => $"{x.FullKey}: {x.Type}").ToArray();

        // Assert
        await _verify.String(output.Join());
    }

    [Test]
    public void ObjectTree_ValidateValue()
    {
        // Arrange
        ObjectTree objectTree = ObjectTree.Create<ExampleClass>();

        // Act
        ObjectTreeProperty[] properties = objectTree.FlattenProperties().ToArray();
        string[] errors = properties
            .SelectMany(x => x.ValidateValue())
            .ToArray();

        // Assert
        Assert.Multiple(async () =>
        {
            Assert.That(errors.Length, Is.EqualTo(4));
            await _verify.String(errors.Join());
        });
    }
}
