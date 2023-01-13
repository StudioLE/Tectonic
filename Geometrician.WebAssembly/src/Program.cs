using Geometrician.Components.Composition;
using Geometrician.Components.Execution;
using Geometrician.Components.Scripts;
using Geometrician.Components.Shared;
using Geometrician.Components.Visualization;
using Geometrician.Core.Execution;
using Geometrician.WebAssembly;
using Lineweights.Core.Storage;
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
builder.Services.AddScoped<CompositionState>();

// Inject Blazor WebAssembly services
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new(builder.HostEnvironment.BaseAddress)
});
await builder.Build().RunAsync();
