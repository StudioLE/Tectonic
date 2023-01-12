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

    public static Action<HostBuilderContext, IServiceCollection>? ConfigureServices { get; set; }

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
    /// Dispose of the host and set _instance to null.
    /// </summary>
    internal static void Reset()
    {
        _host?.Dispose();
        lock (_lock)
            _instance = null;
    }

    private static IHostBuilder CreateBuilder()
    {
        return Host
            .CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddLogging(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Debug));

                // Abstract services

                services.AddTransient<IActivityFactory, StaticMethodActivityFactory>();
                services.AddTransient<IStorageStrategy, FileStorageStrategy>();
                services.AddSingleton<IVisualizationStrategy, VisualizeWithGeometricianServer>();

                // Concrete services

                services.AddTransient<AssetFactoryProvider>();
                services.AddTransient<VisualizationConfiguration>(_ => new VisualizationConfiguration()
                    .RegisterAssetFactory<Model, GlbAssetFactory>()
                    .RegisterAssetFactory<ExternalAsset, AssetFactory>()
                    .RegisterAssetFactory<InternalAsset, AssetFactory>()
                    .RegisterAssetFactory<Model, CsvElementTypesAssetFactory>()
                    .RegisterAssetFactory<Model, JsonAssetFactory>());

                services.AddTransient<GlbAssetFactory>();
                services.AddTransient<AssetFactory>();
                services.AddTransient<CsvElementTypesAssetFactory>();
                services.AddTransient<JsonAssetFactory>();
                // IActivityFactory
                services.AddTransient<StaticMethodActivityFactory>();
                // IStorageStrategy
                services.AddTransient<BlobStorageStrategy>();
                services.AddTransient<FileStorageStrategy>();
                // IVisualizationStrategy
                services.AddSingleton<VisualizeWithGeometricianServer>();
                services.AddTransient<VisualizeAsFile>();

                ConfigureServices?.Invoke(_, services);
            });
    }

    /// <summary>
    /// Create a new instance of the <see cref="IServiceProvider"/>.
    /// </summary>
    private static IServiceProvider CreateInstance()
    {
        _host = CreateBuilder().Build();
        return _host.Services;
    }

    // TODO: When is it appropriate to Services.Dispose()?
    public static void Dispose()
    {
        _host?.Dispose();
    }
}
