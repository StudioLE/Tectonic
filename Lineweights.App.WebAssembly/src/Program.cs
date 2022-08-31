using Lineweights.App.Core.Scripts;
using Lineweights.App.Core.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Lineweights.App.WebAssembly;
using Lineweights.Workflows.Assets;

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

// Inject Lineweights scoped services
builder.Services.AddScoped<RunnerState>();

// Inject Blazor WebAssembly services
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new(builder.HostEnvironment.BaseAddress)
});
await builder.Build().RunAsync();
