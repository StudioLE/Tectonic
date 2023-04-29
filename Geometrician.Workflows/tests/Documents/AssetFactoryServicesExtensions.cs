using Geometrician.Core.Assets;
using Geometrician.Core.Storage;
using Geometrician.Workflows.Assets;
using Geometrician.Workflows.Configuration;
using Geometrician.Workflows.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Geometrician.Workflows.Tests.Documents;

public static class AssetFactoryServicesExtensions
{
    public static IHostBuilder AddAssetFactoryServices(this IHostBuilder builder)
    {
        return builder.ConfigureServices(services =>
        {
            services.AddLogging(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Debug));

            services.AddTransient<IStorageStrategy, FileStorageStrategy>();

            services.AddTransient<AssetFactoryResolver>(x => new AssetFactoryResolverBuilder(x)
                .Register<Model, GlbAssetFactory>()
                .Register<ExternalAsset, AssetFactory>()
                .Register<InternalAsset, AssetFactory>()
                .Register<Model, CsvElementTypesAssetFactory>()
                .Register<Model, JsonAssetFactory>()
                .Build());

            services.AddTransient<GlbAssetFactory>();
            services.AddTransient<AssetFactory>();
            services.AddTransient<CsvElementTypesAssetFactory>();
            services.AddTransient<JsonAssetFactory>();
        });
    }
}
