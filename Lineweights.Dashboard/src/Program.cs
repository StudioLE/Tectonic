using System.Diagnostics;
using Lineweights.Dashboard.Hubs;
using Lineweights.Dashboard.Scripts;
using Lineweights.Dashboard.States;
using Lineweights.Workflows.Results;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add Blazor services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();



// Inject states
builder.Services.AddScoped<ModelViewerFacade>();
builder.Services.AddScoped<SignalRState>();
builder.Services.AddScoped<TestRunnerState>();

// Add SignalR services
// https://docs.microsoft.com/en-us/aspnet/core/blazor/tutorials/signalr-blazor?view=aspnetcore-6.0&tabs=visual-studio&pivots=server
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

// Use Newtonsoft's Json.NET for signal serialisation
// https://docs.microsoft.com/en-us/aspnet/core/migration/22-to-30?view=aspnetcore-6.0&tabs=visual-studio#use-newtonsoftjson-in-an-aspnet-core-30-signalr-project
builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();

// Add JSRuntime
//builder.Services.AddSingleton(serviceProvider => (IJSUnmarshalledRuntime)serviceProvider.GetRequiredService<IJSRuntime>());

builder.Services.Configure<StaticFileOptions>(options =>
{
    options.ContentTypeProvider = new FileExtensionContentTypeProvider
    {
        Mappings =
        {
            [".gltf"] = "model/gltf+json",
            [".glb"] = "model/gltf-binary",
            [".bin"] = "application/octet-stream"
        }
    };
});

// Launch Azurite
Process? azurite = Process.Start(new ProcessStartInfo
{
    FileName = "npm",
    Arguments = "run azurite",
    UseShellExecute = true
});


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();

//app.UsePathBase();

app.UseRouting();

app.MapBlazorHub();

// Add SignalR hub
app.MapHub<SignalRHub>(SendToDashboard.HubPath);

app.MapFallbackToPage("/_Host");

app.Run();
