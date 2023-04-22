using Geometrician.Diagnostics.NUnit.Visualization;
using Geometrician.Drawings.Samples;
using NUnit.Framework;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Geometrician.Drawings.Tests.Samples;

internal sealed class SheetSampleTest
{
    private readonly Verify _verify = new(new NUnitVerifyContext());
    private readonly Visualize _visualize = new();
    private Model _model = new();

    [Test]
    public void SheetSample_Execute()
    {
        // Arrange
        SheetSample.ViewInputs viewInputs = new();
        SheetSample.SheetInputs sheetInputs = new();
        SheetSample.ArrangementInputs arrangementInputs = new();

        // Act
        SheetSample.Outputs outputs = SheetSample.Execute(viewInputs, sheetInputs, arrangementInputs);

        // Preview
        _model.AddElements(outputs.Model.Elements.Values);

        // Assert
        // IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        // await _verify.ElementsByBounds(components);
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
