using Geometrician.Core.Samples;
using Lineweights.Core.Assets;
using Lineweights.Diagnostics.Samples;
using Lineweights.Drawings;
using Lineweights.Workflows.NUnit.Visualization;

namespace Geometrician.Core.Tests;

internal sealed class SamplesTests
{
    private readonly Visualize _visualize = new();
    private Model _model = new();

    [Test]
    public void AssetTypes([Values] Scenes.Name name, [Values] bool includeAssetsInModel)
    {
        // Arrange
        AssetTypes.Inputs inputs = new()
        {
            Scene = name,
            IncludeAssetsInModel = includeAssetsInModel
        };

        // Act
        AssetTypes.Outputs outputs = Geometrician.Core.Samples.AssetTypes.Execute(inputs);

        // Preview
        _model = outputs.Model;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(outputs.Model.AllElementsOfType<View>().Count(), Is.EqualTo(3), "View count");
            Assert.That(outputs.Model.AllElementsOfType<Sheet>().Count(), Is.EqualTo(1), "Sheet count");
            if (inputs.IncludeAssetsInModel)
            {
                Assert.That(outputs.Model.AllElementsOfType<ExternalAsset>().Count(), Is.EqualTo(1), "ExternalAsset count");
                Assert.That(outputs.Model.AllElementsOfType<InternalAsset>().Count(), Is.EqualTo(1), "InternalAsset count");
                Assert.That(outputs.ExternalAsset, Is.Null, "ExternalAsset");
                Assert.That(outputs.InternalAsset, Is.Null, "InternalAsset");
            }
            else
            {
                Assert.That(outputs.Model.AllElementsOfType<ExternalAsset>().Count(), Is.EqualTo(0), "ExternalAsset count");
                Assert.That(outputs.Model.AllElementsOfType<InternalAsset>().Count(), Is.EqualTo(0), "InternalAsset count");
                Assert.That(outputs.ExternalAsset, Is.Not.Null, "ExternalAsset");
                Assert.That(outputs.InternalAsset, Is.Not.Null, "InternalAsset");
            }
        });
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
