using Lineweights.App.Core.Scripts;
using Lineweights.App.Core.Shared;
using Lineweights.App.Server.Hubs;
using Lineweights.Workflows.Assets;
using Lineweights.Workflows.Execution;
using Lineweights.Workflows.Visualization;
using Microsoft.AspNetCore.ResponseCompression;

// Create Builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Inject Blazor server side services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Inject Lineweights singleton services
builder.Services.AddSingleton<GlobalState>();

// Inject Lineweights scoped services
builder.Services.AddScoped<ActivityBuilder>();
builder.Services.AddScoped<ObjectUrlStorage>();
builder.Services.AddScoped<IStorageStrategy, ObjectUrlStorageStrategy>();
builder.Services.AddScoped<ModelViewer>();

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
app.MapHub<SignalRHub>(VisualizeInServerApp.HubPath);
app.MapFallbackToPage("/_Host");
app.Run();
