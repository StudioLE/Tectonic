using Geometrician.Cascade.Components.Composition;
using Geometrician.Cascade.Components.Scripts;
using Geometrician.Cascade.Components.Shared;
using Geometrician.Cascade.Components.Visualization;
using Geometrician.Cascade.WebAssembly;
using Geometrician.Core.Assets;
using Geometrician.Core.Storage;
using Geometrician.Drawings;
using Geometrician.Drawings.Samples;
using Geometrician.Flex.Samples;
using Geometrician.IFC;
using Geometrician.PDF;
using Geometrician.SVG;
using Geometrician.Workflows.Assets;
using Geometrician.Workflows.Configuration;
using Geometrician.Workflows.Execution;
using Geometrician.Workflows.Samples;
using Geometrician.Workflows.Visualization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

// Create Builder
WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add Blazor WebAssembly root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add MudBlazor
builder.Services.AddMudServices();

// Inject general Geometrician services
builder.Services.AddSingleton<VisualizationState>();
builder.Services.AddTransient<ModelViewer>();
builder.Services.AddScoped<CompositionState>();
builder.Services.AddScoped<CommunicationState>();
builder.Services.AddTransient<ObjectUrlStorage>();
builder.Services.AddTransient<DisplayState>();
builder.Services.AddTransient<AssemblyResolver>(_ => new AssemblyResolverBuilder()
    .Register(typeof(SheetSample).Assembly)
    .Register(typeof(AssetTypes).Assembly)
    .Register(typeof(WallFlemishBond).Assembly)
    .Build());
builder.Services.AddTransient<AssetFactoryResolver>(services => new AssetFactoryResolverBuilder(services)
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
    .Build());
builder.Services.AddTransient<ViewerComponentResolver>(_ => new ViewerComponentResolverBuilder()
    .Register("application/pdf", typeof(ObjectViewerComponent))
    .Register("model/gltf-binary", typeof(ThreeViewerComponent))
    .Register("text/csv", typeof(TableViewerComponent))
    .Register("text/plain", typeof(TextViewerComponent))
    .Build());
builder.Services.AddTransient<VisualizationConfiguration>();
builder.Services.AddTransient<GlbAssetFactory>();
builder.Services.AddTransient<AssetFactory>();
builder.Services.AddTransient<SvgAssetFactory<Sheet>>();
builder.Services.AddTransient<SvgAssetFactory<View>>();
builder.Services.AddTransient<CsvElementTypesAssetFactory>();
builder.Services.AddTransient<PdfAssetFactory<Sheet>>();
builder.Services.AddTransient<PdfAssetFactory<View>>();
builder.Services.AddTransient<IfcAssetFactory>();
builder.Services.AddTransient<JsonAssetFactory>();

// Inject environment specific Geometrician services
builder.Services.AddTransient<IActivityResolver, StaticMethodActivityResolver>();
builder.Services.AddTransient<IStorageStrategy, ObjectUrlStorageStrategy>();

// Inject Blazor WebAssembly services
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new(builder.HostEnvironment.BaseAddress)
});
await builder.Build().RunAsync();
