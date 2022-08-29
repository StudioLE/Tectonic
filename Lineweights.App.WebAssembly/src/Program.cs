using Lineweights.App.Core.Scripts;
using Lineweights.App.Core.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Lineweights.App.WebAssembly;
using Lineweights.Workflows.Containers;
using Lineweights.Workflows.Execution;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Inject
builder.Services.AddScoped<ActivityBuilder>();
builder.Services.AddScoped<GlobalState>();
builder.Services.AddScoped<ModelViewer>();
// TODO: Consider why these need to be Singleton?
builder.Services.AddSingleton<ObjectUrlStorage>();
builder.Services.AddSingleton<IStorageStrategy, ObjectUrlStorageStrategy>();
builder.Services.AddScoped<SignalRState>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

await builder.Build().RunAsync();
