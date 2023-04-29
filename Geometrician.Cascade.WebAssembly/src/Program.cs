using Geometrician.Cascade.Components.Scripts;
using Geometrician.Cascade.Extensions;
using Geometrician.Cascade.WebAssembly;
using Geometrician.Core.Storage;
using Geometrician.Drawings.Samples;
using Geometrician.Flex.Samples;
using Geometrician.Workflows.Samples;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StudioLE.Workflows;
using StudioLE.Workflows.Abstractions;
using StudioLE.Workflows.Providers;

// Create Builder
WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add Blazor WebAssembly root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Inject standard Geometrician services
builder.Services.AddComponentServices();
builder.Services.AddDefaultAssetFactoryServices();
builder.Services.AddDefaultViewerComponentServices();
builder.Services.AddConfigurationServices();
builder.Services.AddTransient<IActivityResolver, ActivityResolver>();

// Inject Blazor WebAssembly specific Geometrician services
builder.Services.AddTransient<IStorageStrategy, ObjectUrlStorageStrategy>();
builder.Services.AddTransient<AssemblyResolver>(_ => new AssemblyResolverBuilder()
    .Register(typeof(SheetSample).Assembly)
    .Register(typeof(AssetTypes).Assembly)
    .Register(typeof(WallFlemishBond).Assembly)
    .Build());
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new(builder.HostEnvironment.BaseAddress)
});
await builder.Build().RunAsync();
