using Geometrician.Cascade.Components.Composition;
using Geometrician.Cascade.Components.Scripts;
using Geometrician.Cascade.Components.Shared;
using Geometrician.Cascade.Components.Visualization;
using Geometrician.Core.Assets;
using Geometrician.Drawings;
using Geometrician.IFC;
using Geometrician.PDF;
using Geometrician.SVG;
using Geometrician.Workflows.Assets;
using Geometrician.Workflows.Configuration;
using Geometrician.Workflows.Visualization;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;

namespace Geometrician.Cascade.Extensions;

public static class HostBuilderExtensions
{
    public static IServiceCollection AddComponentServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
                config.SnackbarConfiguration.RequireInteraction = true;
            })
            .AddSingleton<VisualizationState>()
            .AddTransient<ModelViewer>()
            .AddScoped<CompositionState>()
            .AddScoped<CommunicationState>()
            .AddTransient<ObjectUrlStorage>()
            .AddTransient<DisplayState>();
    }

    public static IServiceCollection AddDefaultAssetFactoryServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddTransient<AssetFactoryResolver>(services => new AssetFactoryResolverBuilder(services)
                .Register<Model, GlbAssetFactory>()
                .Register<ExternalAsset, AssetFactory>()
                .Register<InternalAsset, AssetFactory>()
                .Register<Sheet, SvgAssetFactory<Sheet>>()
                .Register<View, SvgAssetFactory<View>>()
                .Register<Model, CsvElementTypesAssetFactory>()
                .Register<Sheet, PdfAssetFactory<Sheet>>()
                .Register<View, PdfAssetFactory<View>>()
                .Register<Model, IfcAssetFactory>()
                .Register<Model, JsonAssetFactory>()
                .Build())
            .AddTransient<GlbAssetFactory>()
            .AddTransient<AssetFactory>()
            .AddTransient<SvgAssetFactory<Sheet>>()
            .AddTransient<SvgAssetFactory<View>>()
            .AddTransient<CsvElementTypesAssetFactory>()
            .AddTransient<PdfAssetFactory<Sheet>>()
            .AddTransient<PdfAssetFactory<View>>()
            .AddTransient<IfcAssetFactory>()
            .AddTransient<JsonAssetFactory>();
    }

    public static IServiceCollection AddDefaultViewerComponentServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddTransient<ViewerComponentResolver>(_ => new ViewerComponentResolverBuilder()
                .Register("application/pdf", typeof(ObjectViewerComponent))
                .Register("model/gltf-binary", typeof(ThreeViewerComponent))
                .Register("text/csv", typeof(TableViewerComponent))
                .Register("text/plain", typeof(TextViewerComponent))
                .Build());
    }

    public static IServiceCollection AddConfigurationServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddTransient<VisualizationConfiguration>();
    }
}
