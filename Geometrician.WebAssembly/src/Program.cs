using Geometrician.Core.Execution;
using Geometrician.Core.Scripts;
using Geometrician.Core.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Geometrician.WebAssembly;
using Lineweights.Workflows.Assets;
using Lineweights.Workflows.Execution;

// Create Builder
var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add Blazor WebAssembly root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Inject Lineweights singleton services
builder.Services.AddSingleton<AssetState>();
builder.Services.AddSingleton<ObjectUrlStorage>();
builder.Services.AddSingleton<IStorageStrategy, ObjectUrlStorageStrategy>();
builder.Services.AddSingleton<ModelViewer>();
builder.Services.AddSingleton<IActivityFactory, StaticMethodActivityFactory>();

// Inject Lineweights scoped services
builder.Services.AddScoped<RunnerState>();

// Inject Blazor WebAssembly services
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new(builder.HostEnvironment.BaseAddress)
});
await builder.Build().RunAsync();
