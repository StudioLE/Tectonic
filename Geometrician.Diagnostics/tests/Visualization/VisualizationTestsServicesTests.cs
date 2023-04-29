using Geometrician.Core.Storage;
using Geometrician.Workflows.Visualization;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using StudioLE.Workflows.Abstractions;

namespace Geometrician.Diagnostics.Tests.Visualization;

internal sealed class VisualizationTestsServicesTests
{
    private readonly IHost _host = Host
        .CreateDefaultBuilder()
        .AddVisualizationServices()
        .Build();

    [TestCase(typeof(VisualizationConfiguration))]
    [TestCase(typeof(IVisualizationStrategy))]
    [TestCase(typeof(IActivityResolver))]
    [TestCase(typeof(IStorageStrategy))]
    public void VisualizationTestsServices_GetService(Type type)
    {
        // Arrange
        // Act
        object? service = _host.Services.GetService(type);

        // Assert
        Assert.That(service, Is.Not.Null, "Service");
    }
}
