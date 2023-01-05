using Lineweights.Workflows.Hosting;
using Lineweights.Workflows.NUnit.Visualization;
using Lineweights.Workflows.Visualization;
using Microsoft.Extensions.DependencyInjection;

namespace Lineweights.Workflows.Tests.Visualization;

internal sealed class VisualizationTests
{
    private readonly IServiceProvider _services;
    private readonly Model _model;

    public VisualizationTests()
    {
        _services = Services.GetInstance();
        _model = new();
        _model.AddElements(Scenes.Brickwork());
    }

    [Test]
    public async Task VisualizeAsFile_by_DI()
    {
        // Arrange
        VisualizeAsFile strategy = _services.GetRequiredService<VisualizeAsFile>();
        strategy.IsOpenEnabled = false;
        VisualizeRequest request = new()
        {
            Model = _model
        };

        // Act
        await strategy.Execute(request);

        // Assert
        // Assert.That(asset.Children.Count, Is.EqualTo(1), "Children count");
        // string? path = asset.Children.First().Info.Location?.LocalPath;
        // Assert.That(File.Exists(path), "File exists");
    }

    [TestCase(typeof(VisualizeAsFile))]
    [TestCase(typeof(VisualizeWithGeometricianServer))]
    public async Task Visualize(Type type)
    {
        // Arrange
        IServiceProvider services = Services.GetInstance();
        object service = services.GetRequiredService(type);
        if (service is not IVisualizationStrategy strategy)
            throw new($"The service is not an {nameof(IVisualizationStrategy)}");


        // Act
        Visualize visualize = new(strategy);
        visualize.Queue(_model);
        await visualize.Execute();

        // Assert
        Assert.That(visualize._strategy, Is.TypeOf(type), "Strategy");
    }

    [Test]
    [Explicit("Requires Azurite")]
    [Category("Requires Azurite")]
    public async Task VisualizeWithGeometricianServer_by_DI()
    {
        // Arrange
        VisualizeWithGeometricianServer strategy = _services.GetRequiredService<VisualizeWithGeometricianServer>();
        VisualizeRequest request = new()
        {
            Model = _model
        };

        // Act
        await strategy.Execute(request);

        // Assert
        // Assert.That(asset.Children.Count, Is.EqualTo(1), "Children count");
    }
}
