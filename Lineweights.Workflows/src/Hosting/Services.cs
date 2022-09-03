using Lineweights.Core.Documents;
using Lineweights.Workflows.Documents;
using Lineweights.Workflows.Execution;
using Lineweights.Workflows.Visualization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lineweights.Workflows.Hosting;

public static class Services
{
    private static readonly object _lock = new();
    private static IServiceProvider? _instance;
    private static IHost? _host;

    /// <summary>
    /// The <see cref="IHostBuilder"/> used to create the <see cref="IServiceProvider"/> instance.
    /// </summary>
    public static IHostBuilder Builder { get; set; } = DefaultBuilder();

    /// <summary>
    /// Get a singleton instance.
    /// </summary>
    public static IServiceProvider GetInstance()
    {
        if (_instance is not null)
            return _instance;

        lock (_lock)
            _instance ??= CreateInstance();

        return _instance;
    }

    /// <summary>
    /// The default for <see cref="Builder"/>.
    /// </summary>
    private static IHostBuilder DefaultBuilder()
    {
        return Host
            .CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddLogging(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Debug));

                // Abstract services

                services.AddTransient<IActivityFactory, StaticMethodActivityFactory>();
                services.AddTransient<IAssetBuilder, AssetBuilder>();
                services.AddTransient<IStorageStrategy, FileStorageStrategy>();
                services.AddSingleton<IVisualizationStrategy, VisualizeWithGeometricianServer>();

                // Concrete services

                services.AddSingleton<GeometricianService>();
                // IActivityFactory
                services.AddTransient<StaticMethodActivityFactory>();
                // IStorageStrategy
                services.AddTransient<BlobStorageStrategy>();
                services.AddTransient<FileStorageStrategy>();
                // IVisualizationStrategy
                services.AddSingleton<VisualizeWithGeometricianServer>();
                services.AddTransient<VisualizeAsFile>();
            });
    }

    /// <summary>
    /// The method used to construct an instance if it does not exist.
    /// </summary>
    private static IServiceProvider CreateInstance()
    {
        _host = Builder.Build();
        return _host.Services;
    }

    // TODO: When is it appropriate to Services.Dispose()?
    public static void Dispose()
    {
        _host?.Dispose();
    }
}
