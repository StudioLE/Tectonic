using Geometrician.Core.Execution;
using Geometrician.Core.Scripts;
using Geometrician.Core.Shared;
using Geometrician.Server.Hubs;
using Lineweights.Core.Documents;
using Lineweights.Workflows.Documents;
using Lineweights.Workflows.Execution;
using Lineweights.Workflows.Visualization;
using Microsoft.AspNetCore.ResponseCompression;

// Create Builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Inject Blazor server side services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();


// Inject Geometrician services
builder.Services.AddSingleton<AssetState>();
builder.Services.AddTransient<ModelViewer>();
builder.Services.AddScoped<RunnerState>();
builder.Services.AddTransient<ObjectUrlStorage>();

// Inject Lineweights services
builder.Services.AddTransient<IActivityFactory, StaticMethodActivityFactory>();
builder.Services.AddTransient<IAssetBuilder, AssetBuilder>();
builder.Services.AddTransient<IStorageStrategy, BlobStorageStrategy>();

// Use response compression for SignalR
// https://docs.microsoft.com/en-us/aspnet/core/blazor/tutorials/signalr-blazor?view=aspnetcore-6.0&tabs=visual-studio&pivots=server
builder.Services.AddResponseCompression(opts => opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
    new[]
    {
        "application/octet-stream"
    }));

// Use Newtonsoft's Json.NET for signal serialisation
// https://docs.microsoft.com/en-us/aspnet/core/migration/22-to-30?view=aspnetcore-6.0&tabs=visual-studio#use-newtonsoftjson-in-an-aspnet-core-30-signalr-project
builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();

// Build application
WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");
app.UseStaticFiles();

//app.UsePathBase();
app.UseRouting();
app.MapBlazorHub();
app.MapHub<SignalRHub>(GeometricianService.HubPath);
app.MapFallbackToPage("/_Host");
app.MapFallbackToPage("/run/{AssemblyKey?}/{ActivityKey?}", "/_Host");
app.Run();
