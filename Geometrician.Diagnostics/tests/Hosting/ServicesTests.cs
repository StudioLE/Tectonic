using Geometrician.Workflows.Execution;
using Geometrician.Workflows.Visualization;
using Geometrician.Core.Storage;
using Geometrician.Diagnostics.Hosting;
using NUnit.Framework;

namespace Geometrician.Diagnostics.Tests.Hosting;

internal sealed class ServicesTests
{
    [TestCase(typeof(VisualizationConfiguration))]
    [TestCase(typeof(IVisualizationStrategy))]
    [TestCase(typeof(IActivityResolver))]
    [TestCase(typeof(IStorageStrategy))]
    public void Services_GetService(Type type)
    {
        // Arrange
        IServiceProvider services = Services.GetInstance();

        // Act
        object? service = services.GetService(type);

        // Assert
        Assert.That(service, Is.Not.Null, "Service");
    }
}
