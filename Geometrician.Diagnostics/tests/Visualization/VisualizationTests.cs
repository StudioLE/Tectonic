using Geometrician.Diagnostics.NUnit.Visualization;
using Geometrician.Diagnostics.Samples;
using Geometrician.Diagnostics.Visualization;
using Geometrician.Workflows.Visualization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace Geometrician.Diagnostics.Tests.Visualization;

internal sealed class VisualizationTests
{
    private readonly IHost _host;
    private readonly Model _model;

    public VisualizationTests()
    {
        _host = Host
            .CreateDefaultBuilder()
            .AddVisualizationServices()
            .Build();
        _model = new();
        _model.AddElements(Scenes.Brickwork());
    }

    [Test]
    public async Task VisualizeAsFile_by_DI()
    {
        // Arrange
        VisualizeAsFile strategy = _host.Services.GetRequiredService<VisualizeAsFile>();
        strategy.IsOpenEnabled = false;
        VisualizeRequest request = new()
        {
            Model = _model
        };

        // Act
        await strategy.Execute(request);

        // Assert
        // Assert.That(asset.Children.Count, Is.EqualTo(1), "Children count");
        // string? path = asset.Children.First().Location?.LocalPath;
        // Assert.That(File.Exists(path), "File exists");
    }

    [TestCase(typeof(VisualizeAsFile))]
    [TestCase(typeof(VisualizeWithGeometricianServer))]
    public async Task Visualize(Type type)
    {
        // Arrange
        object service = _host.Services.GetRequiredService(type);
        if (service is not IVisualizationStrategy strategy)
            throw new($"The service is not an {nameof(IVisualizationStrategy)}");
        if(service is VisualizeAsFile visualizeAsFile)
            visualizeAsFile.IsOpenEnabled = false;

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
        VisualizeWithGeometricianServer strategy = _host.Services.GetRequiredService<VisualizeWithGeometricianServer>();
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
