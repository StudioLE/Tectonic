using Geometrician.Components.Composition;
using Geometrician.Components.Scripts;
using Geometrician.Components.Shared;
using Geometrician.Components.Visualization;
using Geometrician.Core.Assets;
using Geometrician.Core.Configuration;
using Geometrician.Core.Execution;
using Geometrician.Core.Storage;
using Geometrician.Core.Visualization;
using Lineweights.Core.Assets;
using Lineweights.Core.Storage;
using Lineweights.Drawings;
using Lineweights.IFC;
using Lineweights.PDF;
using Lineweights.SVG;
using MudBlazor;
using MudBlazor.Services;

// Create Builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Inject Blazor server side services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add MudBlazor
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
    config.SnackbarConfiguration.RequireInteraction = true;
});

// Inject Geometrician configuration
builder.Services.Configure<ActivitiesOptions>(
    builder.Configuration.GetSection(ActivitiesOptions.Section));

// Inject Geometrician services
builder.Services.AddSingleton<VisualizationState>();
builder.Services.AddTransient<ModelViewer>();
builder.Services.AddScoped<CompositionState>();
builder.Services.AddScoped<CommunicationState>();
builder.Services.AddTransient<ObjectUrlStorage>();
builder.Services.AddTransient<DisplayState>();
builder.Services.AddTransient<AssemblyResolver>();
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

// Inject Lineweights services
builder.Services.AddTransient<IActivityResolver, StaticMethodActivityResolver>();
builder.Services.AddTransient<IStorageStrategy, BlobStorageStrategy>();

// Build application
WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");
app.UseStaticFiles();

//app.UsePathBase();
app.UseRouting();
app.UseEndpoints(endpoint =>
{
    endpoint.MapControllers();
    endpoint.MapBlazorHub();
    endpoint.MapFallbackToPage("/_Host");
    endpoint.MapFallbackToPage("/run/{AssemblyKey?}/{ActivityKey?}", "/_Host");
});
app.Run();
