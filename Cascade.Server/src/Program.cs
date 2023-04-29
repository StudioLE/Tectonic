using Cascade.Extensions;
using Geometrician.Core.Storage;
using Cascade.Assets.Storage;
using Cascade.Workflows;
using Cascade.Workflows.Abstractions;
using Cascade.Workflows.Providers;

// Create Builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Inject Blazor server side services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Inject configuration
// TODO: Refactor configuration
builder.Services.Configure<ActivitiesOptions>(
    builder.Configuration.GetSection(ActivitiesOptions.Section));

// Inject standard Geometrician services
builder.Services.AddComponentServices();
builder.Services.AddDefaultAssetFactoryServices();
builder.Services.AddDefaultViewerComponentServices();
builder.Services.AddConfigurationServices();
builder.Services.AddTransient<IActivityResolver, ActivityResolver>();

// Inject Blazor Server specific services
builder.Services.AddTransient<IStorageStrategy, BlobStorageStrategy>();
builder.Services.AddTransient<AssemblyResolver>();

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
