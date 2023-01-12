using Geometrician.Core.Execution;
using Geometrician.Core.Scripts;
using Geometrician.Core.Shared;
using Geometrician.Core.Visualization;
using Lineweights.Core.Documents;
using Lineweights.Drawings;
using Lineweights.IFC;
using Lineweights.PDF;
using Lineweights.SVG;
using Lineweights.Workflows.Documents;
using Lineweights.Workflows.Execution;
using Lineweights.Workflows.Visualization;
using MudBlazor.Services;

// Create Builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Inject Blazor server side services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add MudBlazor
builder.Services.AddMudServices();

// Inject Geometrician services
builder.Services.AddSingleton<VisualizationState>();
builder.Services.AddTransient<ModelViewer>();
builder.Services.AddScoped<ExecutionState>();
builder.Services.AddTransient<ObjectUrlStorage>();
builder.Services.AddTransient<DisplayState>();
builder.Services.AddTransient<AssetFactoryProvider>();
builder.Services.AddTransient<ViewerComponentProvider>();
builder.Services.AddTransient<VisualizationConfiguration>(_ => new VisualizationConfiguration()
    .RegisterAssetFactory<Model, GlbAssetFactory>()
    .RegisterAssetFactory<ExternalAsset, AssetFactory>()
    .RegisterAssetFactory<InternalAsset, AssetFactory>()
    .RegisterAssetFactory<Sheet, SvgAssetFactory<Sheet>>()
    .RegisterAssetFactory<View, SvgAssetFactory<View>>()
    .RegisterAssetFactory<Model, CsvElementTypesAssetFactory>()
    .RegisterAssetFactory<Sheet, PdfAssetFactory<Sheet>>()
    .RegisterAssetFactory<View, PdfAssetFactory<View>>()
    .RegisterAssetFactory<Model, IfcAssetFactory>()
    .RegisterAssetFactory<Model, JsonAssetFactory>()
    .RegisterContentType("application/pdf", typeof(ObjectViewerComponent))
    .RegisterContentType("model/gltf-binary", typeof(ThreeViewerComponent))
    .RegisterContentType("text/csv", typeof(TableViewerComponent))
    .RegisterContentType("text/plain", typeof(TextViewerComponent)));
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
builder.Services.AddTransient<IActivityFactory, StaticMethodActivityFactory>();
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
