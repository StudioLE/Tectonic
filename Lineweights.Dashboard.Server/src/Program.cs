using Lineweights.Dashboard.Core.Scripts;
using Lineweights.Dashboard.Core.Shared;
using Lineweights.Dashboard.Server.Hubs;
using Lineweights.Workflows.Containers;
using Lineweights.Workflows.Execution;
using Lineweights.Workflows.Results;
using Microsoft.AspNetCore.ResponseCompression;

namespace Lineweights.Dashboard.Server;

public class Program
{
    private readonly WebApplicationBuilder _builder;

    private Program(string[] args)
    {
        _builder = WebApplication.CreateBuilder(args);
        InjectServices();
        RunApp();
    }

   private void InjectServices()
    {
        // Add Blazor services to the container.
        _builder.Services.AddRazorPages();
        _builder.Services.AddServerSideBlazor();

        // Inject
        _builder.Services.AddScoped<ActivityBuilder>();
        _builder.Services.AddScoped<GlobalState>();
        _builder.Services.AddScoped<ModelViewer>();
        _builder.Services.AddScoped<ObjectUrlStorage>();
        _builder.Services.AddScoped<IStorageStrategy, ObjectUrlStorageStrategy>();
        _builder.Services.AddScoped<SignalRState>();

        // Add SignalR services
        // https://docs.microsoft.com/en-us/aspnet/core/blazor/tutorials/signalr-blazor?view=aspnetcore-6.0&tabs=visual-studio&pivots=server
        _builder.Services.AddResponseCompression(opts => opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { "application/octet-stream" }));

        // Use Newtonsoft's Json.NET for signal serialisation
        // https://docs.microsoft.com/en-us/aspnet/core/migration/22-to-30?view=aspnetcore-6.0&tabs=visual-studio#use-newtonsoftjson-in-an-aspnet-core-30-signalr-project
        _builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();
    }

   private void RunApp()
    {
        WebApplication app = _builder.Build();
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
            app.UseExceptionHandler("/Error");
        app.UseStaticFiles();
        //app.UsePathBase();
        app.UseRouting();
        app.MapBlazorHub();
        app.MapHub<SignalRHub>(SendToDashboard.HubPath);
        app.MapFallbackToPage("/_Host");
        app.Run();
    }

   public static void Main(string[] args)
   {
       Program program = new(args);
   }
}
