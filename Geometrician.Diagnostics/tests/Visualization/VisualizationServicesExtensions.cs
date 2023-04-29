using Geometrician.Core.Assets;
using Geometrician.Core.Storage;
using Geometrician.Diagnostics.Visualization;
using Geometrician.Workflows.Assets;
using Geometrician.Workflows.Configuration;
using Geometrician.Workflows.Storage;
using Geometrician.Workflows.Visualization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudioLE.Workflows.Abstractions;
using StudioLE.Workflows.StaticMethodActivities;

namespace Geometrician.Diagnostics.Tests.Visualization;

public static class VisualizationServicesExtensions
{
    public static IHostBuilder AddVisualizationServices(this IHostBuilder builder)
    {
        return builder.ConfigureServices((_, services) =>
        {
            services.AddLogging(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Debug));

            // Abstract services

            services.AddTransient<IActivityResolver, StaticMethodActivityResolver>();
            services.AddTransient<IStorageStrategy, FileStorageStrategy>();
            services.AddSingleton<IVisualizationStrategy, VisualizeWithGeometricianServer>();

            // Concrete services

            services.AddTransient<AssetFactoryResolver>(x => new AssetFactoryResolverBuilder(x)
                .Register<Model, GlbAssetFactory>()
                .Register<ExternalAsset, AssetFactory>()
                .Register<InternalAsset, AssetFactory>()
                .Register<Model, CsvElementTypesAssetFactory>()
                .Register<Model, JsonAssetFactory>()
                .Build());
            services.AddTransient<VisualizationConfiguration>();

            services.AddTransient<GlbAssetFactory>();
            services.AddTransient<AssetFactory>();
            services.AddTransient<CsvElementTypesAssetFactory>();
            services.AddTransient<JsonAssetFactory>();
            // IActivityResolver
            services.AddTransient<StaticMethodActivityResolver>();
            // IStorageStrategy
            services.AddTransient<BlobStorageStrategy>();
            services.AddTransient<FileStorageStrategy>();
            // IVisualizationStrategy
            services.AddSingleton<VisualizeWithGeometricianServer>();
            services.AddTransient<VisualizeAsFile>();
        });
    }
}
