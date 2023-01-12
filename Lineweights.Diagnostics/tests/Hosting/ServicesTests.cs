using Geometrician.Core.Execution;
using Geometrician.Core.Visualization;
using Lineweights.Core.Storage;
using Lineweights.Diagnostics.Hosting;

namespace Lineweights.Diagnostics.Tests.Hosting;

internal sealed class ServicesTests
{
    [TestCase(typeof(VisualizationConfiguration))]
    [TestCase(typeof(IVisualizationStrategy))]
    [TestCase(typeof(IActivityFactory))]
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
