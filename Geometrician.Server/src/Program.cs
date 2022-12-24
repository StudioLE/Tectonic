using Geometrician.Core.Execution;
using Geometrician.Core.Scripts;
using Geometrician.Core.Shared;
using Lineweights.Core.Documents;
using Lineweights.Workflows.Documents;
using Lineweights.Workflows.Execution;
using MudBlazor.Services;

// Create Builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Inject Blazor server side services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add MudBlazor
builder.Services.AddMudServices();

// Inject Geometrician services
builder.Services.AddSingleton<AssetState>();
builder.Services.AddTransient<ModelViewer>();
builder.Services.AddScoped<RunnerState>();
builder.Services.AddTransient<ObjectUrlStorage>();
builder.Services.AddTransient<DisplayState>();

// Inject Lineweights services
builder.Services.AddTransient<IActivityFactory, StaticMethodActivityFactory>();
builder.Services.AddTransient<IAssetBuilder, AssetBuilder>();
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
