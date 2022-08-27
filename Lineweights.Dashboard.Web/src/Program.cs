using Lineweights.Dashboard.Core.Scripts;
using Lineweights.Dashboard.Core.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Lineweights.Dashboard.Web;
using Lineweights.Workflows.Execution;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Inject
builder.Services.AddScoped<ActivityBuilder>();
builder.Services.AddScoped<ResultsState>();
builder.Services.AddScoped<ModelViewerFacade>();
builder.Services.AddScoped<SignalRState>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

await builder.Build().RunAsync();
