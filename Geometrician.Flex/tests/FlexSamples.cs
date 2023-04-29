using Geometrician.Diagnostics;
using Geometrician.Diagnostics.NUnit.Visualization;
using Geometrician.Flex.Samples;
using NUnit.Framework;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Geometrician.Flex.Tests;

internal sealed class FlexSamples
{
    private readonly Verify _verify = new(new NUnitVerifyContext());
    private readonly Visualize _visualize = new();
    private Model _model = new();

    [Test]
    public async Task Flex2d_Sample()
    {
        // Arrange
        Flex2dSample.Inputs inputs = new();

        // Act
        Flex2dSample.Outputs outputs = await new Flex2dSample().Execute(inputs);

        // Preview
        _model.AddElements(outputs.Model.Elements.Values);

        // Assert
        Assert.That(outputs, Is.Not.Null, "Outputs");
        Assert.That(outputs.Model.Elements.Count, Is.EqualTo(526), "Outputs model count");
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        await _verify.ElementsByBounds(components);
    }

    [TearDown]
    public void TearDown()
    {
        _visualize.Queue(_model);
        _model = new();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _visualize.Execute();
    }
}
