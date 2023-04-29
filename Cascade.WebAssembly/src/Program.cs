using Cascade.Components.Scripts;
using Cascade.Extensions;
using Cascade.WebAssembly;
using Geometrician.Core.Storage;
using Geometrician.Drawings.Samples;
using Geometrician.Flex.Samples;
using Cascade.Assets.Samples;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Cascade.Workflows;
using Cascade.Workflows.Abstractions;
using Cascade.Workflows.Providers;

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
