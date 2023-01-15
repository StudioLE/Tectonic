using Geometrician.Components.Composition;
using Geometrician.Components.Scripts;
using Geometrician.Components.Shared;
using Geometrician.Components.Visualization;
using Geometrician.Core.Assets;
using Geometrician.Core.Configuration;
using Geometrician.Core.Execution;
using Geometrician.Core.Samples;
using Geometrician.Core.Visualization;
using Geometrician.WebAssembly;
using Lineweights.Core.Assets;
using Lineweights.Core.Storage;
using Lineweights.Drawings;
using Lineweights.Drawings.Samples;
using Lineweights.Flex.Samples;
using Lineweights.IFC;
using Lineweights.PDF;
using Lineweights.SVG;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

// Create Builder
var builder = WebAssemblyHostBuilder.CreateDefault(args);

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
