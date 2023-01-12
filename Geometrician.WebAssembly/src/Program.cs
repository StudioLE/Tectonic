using Geometrician.Core.Execution;
using Geometrician.Core.Scripts;
using Geometrician.Core.Shared;
using Geometrician.Core.Visualization;
using Geometrician.WebAssembly;
using Lineweights.Core.Documents;
using Lineweights.Workflows.Execution;
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

// Inject Lineweights singleton services
builder.Services.AddSingleton<VisualizationState>();
builder.Services.AddSingleton<ObjectUrlStorage>();
builder.Services.AddSingleton<IStorageStrategy, ObjectUrlStorageStrategy>();
builder.Services.AddSingleton<ModelViewer>();
builder.Services.AddSingleton<IActivityFactory, StaticMethodActivityFactory>();
builder.Services.AddTransient<DisplayState>();

// Inject Lineweights scoped services
builder.Services.AddScoped<RunnerState>();

// Inject Blazor WebAssembly services
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new(builder.HostEnvironment.BaseAddress)
});
await builder.Build().RunAsync();
