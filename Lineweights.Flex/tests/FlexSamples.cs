using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Flex.Tests;

[VisualizeAfterTest]
internal sealed class FlexSamples
{
    public Model Model { get; } = new();

    [Test]
    public async Task Flex2d_Sample()
    {
        // Arrange
        Samples.Flex2dSample.ContainerInputs containerInputs = new();
        Samples.Flex2dSample.SequenceInputs firstSequenceInputs = new();
        Samples.Flex2dSample.SequenceInputs secondSequenceInputs = new();
        Samples.Flex2dSample.FlexInputs flexInputs = new();
        Samples.Flex2dSample.DisplayInputs displayInputs = new();

        // Act
        Samples.Flex2dSample.Outputs outputs = Samples.Flex2dSample.Execute(
            containerInputs,
            firstSequenceInputs,
            secondSequenceInputs,
            flexInputs,
            displayInputs);

        // Preview
        Model.AddElements(outputs.Model.Elements.Values);

        // Assert
        Assert.That(outputs, Is.Not.Null, "Outputs");
        Assert.That(outputs.Model.Elements.Count, Is.EqualTo(526), "Outputs model count");
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        await Verify.ElementsByBounds(components);
    }
}
